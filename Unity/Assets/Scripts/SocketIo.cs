using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json.Linq;
using Schema;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnPlayerJoined(Player player);
public delegate void OnPlayerLeft(Player player);
public delegate void OnGameIsStarting();
public delegate void OnGameLobbyTimer(int remainingSeconds);
public delegate void OnGameStarted(Player firstPlayer);
public delegate void OnPlayerRolledDice(Player player, int[] dice);
public delegate void OnPlayerMoved(Player player, int location);
public delegate void OnPlayerTurnChanged(Player player);
public delegate void OnPlayerPlaysAgain(Player player);
public delegate void OnPlayerBalanceChanged(Player player, int balance);
public delegate void OnPlayerSteppedOnChance(Player player, string text);
public delegate void OnPlayerSteppedOnCommunityChest(Player player, string text);
public delegate void OnPropertyOwnerChanged(int propertyIndex, string ownerId);

public class SocketIo : MonoBehaviour
{
    #region Singleton
    public static SocketIo Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
    
    private WebSocket _currentSocket;
    private string _currentSocketId;
    private bool _closed;
    private IDisposable _cookiesSubscription;
    
    public event OnPlayerJoined PlayerJoined;
    public event OnPlayerLeft PlayerLeft;
    public event OnGameIsStarting GameIsStarting;
    public event OnGameLobbyTimer GameLobbyTimer;
    public event OnGameStarted GameStarted;
    public event OnPlayerRolledDice PlayerRolledDice;
    public event OnPlayerMoved PlayerMoved;
    public event OnPlayerTurnChanged PlayerTurnChanged;
    public event OnPlayerPlaysAgain PlayerPlaysAgain;
    public event OnPlayerBalanceChanged PlayerBalanceChanged;
    public event OnPlayerSteppedOnChance PlayerSteppedOnChance;
    public event OnPlayerSteppedOnCommunityChest PlayerSteppedOnCommunityChest;
    public event OnPropertyOwnerChanged PropertyOwnerChanged;
    
    private void Start()
    {
        _cookiesSubscription = ApiWrapper.Cookie.Subscribe(c =>
        {
            Debug.Log("Restarting Socket...");
            InitializeSocket();
        });
    }
    
    private void OnDestroy()
    {
        _cookiesSubscription.Dispose();
        CloseSocket();
    }

    private async void InitializeSocket()
    {
        try
        {
            var response = await ApiWrapper.SocketIo();
            var sid = (string) response["sid"];
            _currentSocket = new WebSocket(ApiWrapper.WS_PROTOCOL + ApiWrapper.URL + "socket.io/?EIO=3&transport=websocket&sid=" + sid);
            _currentSocketId = sid;

            _currentSocket.OnOpen += () =>
            {
                _closed = false;
                Debug.Log("Connection open!");
                _currentSocket.SendText("2probe");
            };

            _currentSocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _currentSocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
                if (!_closed && sid == _currentSocketId) InitializeSocket();
            };

            _currentSocket.OnMessage += (bytes) =>
            {
                if (_closed) return;
                if (_currentSocketId != sid) return;

                try
                {
                    var message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    // Handshake
                    if (message == "3probe")
                    {
                        _currentSocket.SendText("5");
                        
                        // Start ping pong
                        StartCoroutine(SocketIoPing(sid));
                        
                        return;
                    }

                    // Drop unwanted messages
                    if (message.Length <= 3 || message.Substring(0, 2) != "42") return;
                    
                    // Strip "42" from the beginning of the message 
                    message = message.Substring(2);
                    
                    // Process message
                    var array = JArray.Parse(message);
                    Debug.Log(array[0]);
                    HandleSocketIoMessage(array);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            };

            // Waiting for messages
            await _currentSocket.Connect();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async void CloseSocket()
    {
        try
        {
            _closed = true;
            await _currentSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    private IEnumerator SocketIoPing(string sid)
    {
        // Stops when the target socket disconnects
        while (_currentSocket.State == WebSocketState.Open &&
            sid == _currentSocketId &&
            !_closed)
        {
            _currentSocket.SendText("2");
            yield return new WaitForSeconds(15);
        }
    }

    private void HandleSocketIoMessage(JArray array)
    {
        switch ((string) array[0])
        {
            case "playerJoined":
            {
                var player = Player.GetPlayer(array[1]["player"]);
                PlayerJoined?.Invoke(player);
                break;
            }
            case "playerLeft":
            {
                var player = Player.GetPlayerById((string) array[1]["user"]);
                PlayerLeft?.Invoke(player);
                break;
            }
            case "gameIsStarting":
            {
                GameIsStarting?.Invoke();
                break;
            }
            case "gameLobbyTimer":
            {
                var remainingSeconds = (int)array[1]["remainingSeconds"];
                GameLobbyTimer?.Invoke(remainingSeconds);
                break;
            }
            case "gameStarted":
            {
                var firstPlayer = Player.GetPlayerById(array[1]["firstPlayer"].ToString());

                GameStarted?.Invoke(firstPlayer);
                break;
            }
            case "playerRolledDice":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                var dice = ((JArray) array[1]["dice"]).Select(d => (int) d).ToArray();
                PlayerRolledDice?.Invoke(player, dice);
                break;
            }
            case "playerTurnChanged":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                PlayerTurnChanged?.Invoke(player);
                break;
            }
            case "playerPlaysAgain":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                PlayerPlaysAgain?.Invoke(player);
                break;
            }
            case "playerMoved":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                var location = (int) array[1]["location"];
                PlayerMoved?.Invoke(player, location);
                break;
            }
            case "playerBalanceChanged":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                var balance = (int) array[1]["balance"];
                PlayerBalanceChanged?.Invoke(player, balance);
                break;
            }
            case "playerSteppedOnChance":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                var text = array[1]["card"].ToString();
                PlayerSteppedOnChance?.Invoke(player, text);
                break;
            }
            case "playerSteppedOnCommunityChest":
            {
                var player = Player.GetPlayerById(array[1]["user"].ToString());
                var text = array[1]["card"].ToString();
                PlayerSteppedOnCommunityChest?.Invoke(player, text);
                break;
            }
            case "propertyOwnerChanged":
            {
                var propertyIndex = (int) array[1]["propertyIndex"];
                var ownerId = array[1]["ownerId"].ToString();

                var property = CurrentGame().GetPropertyByIndex(propertyIndex);
                property.OwnerId = ownerId;

                PropertyOwnerChanged?.Invoke(propertyIndex, ownerId);
                break;
            }
        }
    }

    private static Game CurrentGame() => Game.Current.Value;
}
