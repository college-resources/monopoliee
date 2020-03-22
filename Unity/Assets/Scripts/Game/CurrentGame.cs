using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CurrentGame : MonoBehaviour
{
    public Dice diceContainer;
    public GameObject chanceCard;
    public GameObject communityChestCard;
    public GameObject nowPlayingPlayer;
    public GameObject ownedProperties;
    public GameObject players;
    public GameObject[] playerPrefabs = new GameObject[4];
    public List<GameObject> playerList;
    public Route route;
    public TextMeshProUGUI statusMessage;
    public TextMeshProUGUI[] playerNameTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] playerBalanceTexts = new TextMeshProUGUI[4];

    private BuyProperty _buyProperty;
    private CameraController _cameraController;
    private CoroutineQueue _queue;
    private Game _game;
    private IDisposable[] _playerBalanceSubscriptions;
    private IDisposable _playerRemovedSubscription;
    private IDisposable _playerTurnChangedSubscription;
    private IDisposable[] _propertyOwnerChangedSubscriptions;
    private readonly Session _session = Session.Instance.Value;
    private readonly SocketIo _socketIo = SocketIo.Instance;
    private readonly Transform[,] _propertyDisplays = new Transform[4, 28];

    private static readonly Vector3[] BottomBarPlayerDisplays = {
        new Vector3(-720, 0, 0),
        new Vector3(-240, 0, 0),
        new Vector3(240, 0, 0),
        new Vector3(720, 0, 0)
    };
    
    private static readonly Vector3[] Offsets = {
        new Vector3(0.15f, 0, 0.15f),
        new Vector3(-0.15f, 0, 0.15f), 
        new Vector3(0.15f, 0, -0.15f),
        new Vector3(-0.15f, 0, -0.15f)
    };

    private void Start()
    {
        _game = Game.Current.Value;
        
        _queue = new CoroutineQueue(this);
        _queue.StartLoop();
        _queue.EnqueueAction(ShowStatusMessage("Press space to roll the dice"));
        
        _cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        _buyProperty = GameObject.Find("BuyUI").GetComponent<BuyProperty>();

        InitializePropertyDisplays();
        InitializeSubscriptions();

        // SocketIO events
        _socketIo.PlayerRolledDice += SocketIoOnPlayerRolledDice;
        _socketIo.PlayerMoved += SocketIoOnPlayerMoved;
        _socketIo.PlayerPlaysAgain += SocketIoOnPlayerPlaysAgain;
        _socketIo.PlayerSteppedOnChance += SocketIoOnPlayerSteppedOnChance;
        _socketIo.PlayerSteppedOnCommunityChest += SocketIoOnPlayerSteppedOnCommunityChest;

        UpdateOwnedProperties();
        UpdateBottomBar();
        SetupPlayers();

        foreach (Transform child in players.transform)
        {
            if (child != players.transform)
            {
                playerList.Add(child.gameObject);
            }
        }

        var playerPlaying = Player.GetPlayerById(Game.Current.Value.CurrentPlayerId.Value);
        UpdateBottomBarPlayerPlaying(playerPlaying);
        
        _cameraController.SetUpCameras();
    }
    
    private void OnDestroy()
    {
        foreach (var subscription in _playerBalanceSubscriptions)
        {
            subscription?.Dispose();
        }
        
        foreach (var subscription in _propertyOwnerChangedSubscriptions)
        {
            subscription?.Dispose();
        }

        _playerRemovedSubscription?.Dispose();
        _playerTurnChangedSubscription?.Dispose();
        
        _socketIo.PlayerRolledDice -= SocketIoOnPlayerRolledDice;
        _socketIo.PlayerMoved -= SocketIoOnPlayerMoved;
        _socketIo.PlayerPlaysAgain -= SocketIoOnPlayerPlaysAgain;
        _socketIo.PlayerSteppedOnChance -= SocketIoOnPlayerSteppedOnChance;
        _socketIo.PlayerSteppedOnCommunityChest -= SocketIoOnPlayerSteppedOnCommunityChest;
    }

    private void PlayerLeft(Player player)
    {
        UpdateBottomBar();
        UpdateOwnedProperties();
    }
    
    private void SocketIoOnPlayerRolledDice(Player player, int[] dice)
    {
        _queue.EnqueueAction(diceContainer.RollTheDice(dice));
        _queue.EnqueueWait(1f);
    }
    
    private void SocketIoOnPlayerMoved(Player player, int location)
    {
        var playerObject = playerList[player.Index];
        _queue.EnqueueAction(playerObject.GetComponent<PlayerMovement>().Move(location));
        _queue.EnqueueWait(1f);
        if (Property.GetPropertyByLocation(location) != null)
        {
            _queue.EnqueueAction(_buyProperty.DisplayCard(location));
        }
    }
    
    private void PlayerTurnChanged(Player player)
    {
        _queue.EnqueueAction(ShowStatusMessage("It's " + player.Name + "'s turn"));
        UpdateBottomBarPlayerPlaying(player);
        _cameraController.FocusCameraOn(player);
    }

    private void SocketIoOnPlayerPlaysAgain(Player player)
    {
        _queue.EnqueueAction(ShowStatusMessage(player.Name + " plays again"));
    }

    private void SocketIoOnPlayerSteppedOnChance(Player player, string text)
    {
        _queue.EnqueueAction(DisplayChanceCard(text));
        _queue.EnqueueWait(1f);
    }
    
    private void SocketIoOnPlayerSteppedOnCommunityChest(Player player, string text)
    {
        _queue.EnqueueAction(DisplayCommunityChestCard(text));
        _queue.EnqueueWait(1f);
    }
    
    private void PropertyOwnerChanged(int propertyIndex, string currentOwnerId, string previousOwnerId)
    {
        Transform propertyComponent;
        Image image;
        Color tempColor;
        
        if (!string.IsNullOrEmpty(previousOwnerId))
        {
            var previousOwner = Player.GetPlayerById(previousOwnerId);
            
            // Update previous owner's property UI
            propertyComponent = _propertyDisplays[previousOwner.Index, propertyIndex];
            image = propertyComponent.GetComponent<Image>();
            tempColor = image.color;
            tempColor.a = 0.4f;
            image.color = tempColor;
        }
        
        var currentOwner = Player.GetPlayerById(currentOwnerId);

        if (currentOwner == null) return;
        
        // Update current owner's property UI
        propertyComponent = _propertyDisplays[currentOwner.Index, propertyIndex];
        image = propertyComponent.GetComponent<Image>();
        tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;
    }

    private void SetupPlayers()
    {
        foreach (var player in _game.Players)
        {
            AddPlayer(player);
        }
    }

    private void AddPlayer(Player player)
    {
        var playerPos = route.childNodeList[player.Position].transform.position + Offsets[player.Index];
        var playerRotation = new Vector3(0, player.Position / 10 * 90, 0);
        var newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.Euler(playerRotation), players.transform);
        newPlayer.GetComponent<PlayerMovement>().offset = Offsets[player.Index];
    }

    private void UpdateBottomBarPlayerPlaying(Player player)
    {
        var currentPlayerIndex = player.Index;
        nowPlayingPlayer.transform.localPosition = BottomBarPlayerDisplays[currentPlayerIndex];
    }
    
    private void UpdateBottomBar()
    {
        if (_game == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            playerNameTexts[i].text = "";
            playerBalanceTexts[i].text = "";
        }
        
        var selfPlayerId = _session.User.Id;
        
        foreach (var player in _game.Players)
        {
            var index = player.Index;
            
            if (player.UserId == selfPlayerId)
            {
                playerNameTexts[index].text = "•" + player.Name + "•";
            }
            else
            {
                playerNameTexts[index].text = player.Name;
            }
            
            playerBalanceTexts[index].text = player.Balance.Value + "ΔΜ";
        }
    }
    
    private void UpdateOwnedProperties()
    {
        var properties = _game.Properties;

        foreach (var player in _game.Players)
        {
            var playerOwnedProperties = ownedProperties.transform.GetChild(player.Index);
            playerOwnedProperties.gameObject.SetActive(true);
            
            foreach (var property in properties)
            {
                var propertyComponent = playerOwnedProperties.Find(property.Location.ToString());
                
                if (propertyComponent == null) continue;
                
                var image = propertyComponent.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = property.OwnerId.Value == player.UserId ? 1f : 0.4f;
                image.color = tempColor;
            }
        }
    }

    private void InitializePropertyDisplays()
    {
        var properties = _game.Properties;
        
        foreach (var player in _game.Players)
        {
            var playerOwnedProperties = ownedProperties.transform.GetChild(player.Index);
            playerOwnedProperties.gameObject.SetActive(true);
            
            foreach (var (property, index) in properties.Select((value, index) => ( value, index )))
            {
                var propertyComponent = playerOwnedProperties.Find(property.Location.ToString());
                _propertyDisplays[player.Index, index] = propertyComponent;
            }
        }
    }

    private void InitializeSubscriptions()
    {
        _playerBalanceSubscriptions = new IDisposable[_game.Seats];
        foreach (var player in _game.Players)
        {
            var index = player.Index;
            _playerBalanceSubscriptions[index] = player.Balance.Skip(1).Select(x => x + "ΔΜ").SubscribeToText(playerBalanceTexts[index]);
        }
        
        _propertyOwnerChangedSubscriptions = new IDisposable[28];
        foreach (var (property, index) in _game.Properties.Select((value, index) => ( value, index )))
        {
            _propertyOwnerChangedSubscriptions[index] = property.OwnerId
                .StartWith(string.Empty)
                .Pairwise()
                .Subscribe(owners => PropertyOwnerChanged(index, owners.Current, owners.Previous));
        }
        
        _playerRemovedSubscription = _game.PlayerRemoved.Subscribe(PlayerLeft);
        _playerTurnChangedSubscription = _game.CurrentPlayerId.Subscribe(id => PlayerTurnChanged(Player.GetPlayerById(id)));
    }
    
    private IEnumerator ShowStatusMessage(string text)
    {
        statusMessage.text = text;
        yield return new WaitForSecondsRealtime(2f);
        statusMessage.text = "";
        chanceCard.SetActive(false);
    }

    private IEnumerator DisplayChanceCard(string text)
    {
        chanceCard.SetActive(true);
        chanceCard.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = text;
        yield return new WaitForSecondsRealtime(5f);
        chanceCard.SetActive(false);
    }
    
    private IEnumerator DisplayCommunityChestCard(string text)
    {
        communityChestCard.SetActive(true);
        communityChestCard.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = text;
        yield return new WaitForSecondsRealtime(5f);
        communityChestCard.SetActive(false);
    }
}
