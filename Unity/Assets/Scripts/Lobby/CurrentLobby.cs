using System;
using System.Collections;
using Schema;
using TMPro;
using UniRx;
using UnityEngine;

public class CurrentLobby : MonoBehaviour
{
    private Game _game;
    private GameManager _gameManager;
    private readonly Session _session = Session.Instance.Value;
    private readonly SocketIo _socketIo = SocketIo.Instance;
    
    public GameObject waitingText;
    public GameObject bottomBar;
    
    private void Start()
    {
        _socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        _socketIo.PlayerLeft += SocketIoOnPlayerLeft;

        _game = GameManager.Instance.Game;
        _gameManager = GameManager.Instance;

        UpdateWaitingText();
        UpdateBottomBar();
        
        _game.Status.Subscribe(status =>
        { 
            if (_game.Status.Value == "running")
            {
                StartCoroutine(GameCountdown());
            }
        });
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

    private IEnumerator GameCountdown()
    {
        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
            
        for (var i = 3; i >= 0; i--)
        {
            waitingTextMeshPro.text = "Game starting in " + i;
            yield return new WaitForSecondsRealtime(1f);
        }
            
        _gameManager.GoToGame(_game);
    }
}
