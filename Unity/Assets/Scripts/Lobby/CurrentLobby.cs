using System;
using Schema;
using TMPro;
using UniRx;
using UnityEngine;

public class CurrentLobby : MonoBehaviour
{
    private Game _game;
    private IDisposable playerAddedSubscription;
    private IDisposable playerRemovedSubscription;
    private IDisposable lobbyTimeSubscription;
    private readonly Session _session = Session.Instance.Value;
    
    public GameObject bottomBar;
    public TextMeshProUGUI waitingText;
    
    private void Start()
    {
        _game = Game.Current.Value;

        playerAddedSubscription = _game.PlayerAdded.Subscribe(PlayerJoined);
        playerRemovedSubscription = _game.PlayerRemoved.Subscribe(PlayerLeft);
        lobbyTimeSubscription = _game.LobbyTime.Skip(1).Subscribe(
            remainingSeconds => { waitingText.text = "Game starting in " + remainingSeconds; }
        );

        UpdateWaitingText();
        UpdateBottomBar();
    }

    private void OnDestroy()
    {
        playerAddedSubscription?.Dispose();
        playerRemovedSubscription?.Dispose();
        lobbyTimeSubscription?.Dispose();
    }

    private void PlayerJoined(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }
    
    private void PlayerLeft(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }

    private void UpdateWaitingText()
    {
        waitingText.text = "Waiting for players " + _game.Players.Count + "/" + _game.Seats;
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
