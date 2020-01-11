using System.Collections;
using System.Collections.Generic;
using Schema;
using TMPro;
using UnityEngine;

public class CurrentGame : MonoBehaviour
{
    public SocketIo socketIo;
    public GameObject bottomBar;
    
    // Start is called before the first frame update
    void Start()
    {
        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        Game game = GameManager.Instance.Game;
        UpdateBottomBar(game);
    }
    
    private void UpdateBottomBar(Game game)
    {
        Transform bottomBarTransform = bottomBar.transform;
        Transform nameTextTransform = bottomBarTransform.Find("PlayerOne").Find("PlayerOneNameText");
        TextMeshProUGUI playerOneNameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
        playerOneNameText.text = game.Players[0].UserId;
        
        Transform nameCurrencyTransform = bottomBarTransform.Find("PlayerOne").Find("PlayerOneCurrencyText");
        TextMeshProUGUI playerOneCurrencyText = nameCurrencyTransform.GetComponent<TextMeshProUGUI>();
        playerOneCurrencyText.text = game.Players[0].Balance.ToString() + "ΔΜ";
    }
}
