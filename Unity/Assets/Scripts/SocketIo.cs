using System;
using System.Linq;
using System.Text;
using NativeWebSocket;
using Newtonsoft.Json.Linq;
using Schema;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnPlayerJoined(Player player);
public delegate void OnPlayerLeft(Player player);
public delegate void OnGameStarted();
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
    public static SocketIo Instance { get; set; }

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
    
    private WebSocket _websocket;
    private bool _closed;
    public event OnPlayerJoined PlayerJoined;
    public event OnPlayerLeft PlayerLeft;
    public event OnGameStarted GameStarted;
    public event OnPlayerRolledDice PlayerRolledDice;
    public event OnPlayerMoved PlayerMoved;
    public event OnPlayerTurnChanged PlayerTurnChanged;
    public event OnPlayerPlaysAgain PlayerPlaysAgain;
    public event OnPlayerBalanceChanged PlayerBalanceChanged;
    public event OnPlayerSteppedOnChance PlayerSteppedOnChance;
    public event OnPlayerSteppedOnCommunityChest PlayerSteppedOnCommunityChest;
    public event OnPropertyOwnerChanged PropertyOwnerChanged;
    
    private async void Start()
    {
        try
        {
            var response = await Upload("socket.io/?EIO=3&transport=polling");

            var sid = (string) response["sid"];

            _websocket = new WebSocket(ApiWrapper.WS_PROTOCOL + ApiWrapper.URL + "socket.io/?EIO=3&transport=websocket&sid=" + sid);

            _websocket.OnOpen += () =>
            {
                _closed = false;
                Debug.Log("Connection open!");
                _websocket.SendText("2probe");
            };

            _websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
                if (!_closed) Start();
            };

            _websocket.OnMessage += (bytes) =>
            {
                if (_closed) return;
                
                try
                {
                    var message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (message == "3probe")
                    {
                        _websocket.SendText("5");
                        return;
                    }

                    if (message.Length <= 3 || message.Substring(0, 2) != "42") return;
                    
                    message = message.Substring(2);
                    var array = JArray.Parse(message);
                    
                    Debug.Log(array[0]);
                    
                    switch ((string) array[0])
                    {
                        case "playerJoined":
                        {
                            var player = Player.GetPlayer(array[1]["player"]);
                            GetCurrentGame().Players.Add(player);
                            PlayerJoined?.Invoke(player);
                            break;
                        }
                        case "playerLeft":
                        {
                            foreach (var player in GetCurrentGame().Players.ToArray())
                            {
                                if (player.UserId != (string) array[1]["user"]) continue;
                                
                                GetCurrentGame().Players.Remove(player);
                                PlayerLeft?.Invoke(player);
                            }
                            break;
                        }
                        case "gameStarted":
                        {
                            var firstPlayer = Player.GetPlayerById(array[1]["firstPlayer"].ToString());
                            GameManager.Instance.Game.UpdateCurrentPlayer(firstPlayer);
                            
                            GameStarted?.Invoke();
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
                            GameManager.Instance.Game.UpdateCurrentPlayer(player);

                            Debug.Log(player.UserId);
                            
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

                            player.Balance = balance;

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

                            var property = GameManager.Instance.Game.GetPropertyByIndex(propertyIndex);
                            property.OwnerId = ownerId;

                            PropertyOwnerChanged?.Invoke(propertyIndex, ownerId);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            };

            // Keep sending messages every 15s
            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 5.0f);
            
            // Waiting for messages
            await _websocket.Connect();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    private void OnDestroy()
    {
        try
        {
            _closed = true;
            CancelInvoke(nameof(SendWebSocketMessage));
            _websocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _websocket.SendText("2");
        }
    }

    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }

    private static Game GetCurrentGame()
    {
        return GameManager.Instance.Game;
    }
    
    private static async UniTask<string> GetTextAsync(UnityWebRequest req)
    {
        var op = await req.SendWebRequest();
        return op.downloadHandler.text;
    }

    private static async UniTask<JToken> Upload(string path)
    {
        var www = UnityWebRequest.Get(ApiWrapper.HTTP_PROTOCOL + ApiWrapper.URL + path);
        
        var resText = await GetTextAsync(www);
        var json = resText.Split(new[] {":0"}, StringSplitOptions.None)[1];
        var response = JToken.Parse(json);
        
        if (www.error != null)
        {
            throw new BadResponseException(www.error, response);
        }
        
        return response;
    }
}
