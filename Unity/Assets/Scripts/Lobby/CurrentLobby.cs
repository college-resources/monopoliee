using System.Collections;
using Schema;
using TMPro;
using UniRx;
using UnityEngine;

public class CurrentLobby : MonoBehaviour
{
    private Game _game;
    private readonly Session _session = Session.Instance.Value;
    private readonly SocketIo _socketIo = SocketIo.Instance;
    
    public GameObject waitingText;
    public GameObject bottomBar;
    
    private void Start()
    {
        _socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        _socketIo.PlayerLeft += SocketIoOnPlayerLeft;

        _game = Game.Current.Value;

        UpdateWaitingText();
        UpdateBottomBar();
        
        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
        _game.LobbyTime.Skip(1).Subscribe(remainingSeconds => { waitingTextMeshPro.text = "Game starting in " + remainingSeconds; });
    }

    private void OnDestroy()
    {
        _socketIo.PlayerJoined -= SocketIoOnPlayerJoined;
        _socketIo.PlayerLeft -= SocketIoOnPlayerLeft;
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }

    private void UpdateWaitingText()
    {
        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
        waitingTextMeshPro.text = "Waiting for players " + _game.Players.Count + "/" + _game.Seats;
    }
    
    private void UpdateBottomBar()
    {
        var selfPlayerId = _session.User.Id;
        var bottomBarTransform = bottomBar.transform;

        if (_game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            typeTextMeshPro.text = "";
        }

        foreach (var player in _game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            if (player.UserId == selfPlayerId)
            {
                nameTextMeshPro.text = "•" + player.Name + "•";
            }
            else
            {
                nameTextMeshPro.text = player.Name;
            }
            
            typeTextMeshPro.text = "Player";
        }
    }
}
