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
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        
        UpdateBottomBar();
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateBottomBar();
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateBottomBar();
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
        var bottomBarTransform = bottomBar.transform;

        if (game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            balanceTextMeshPro.text = "";
        }

        foreach (var player in game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = player.UserId;
            balanceTextMeshPro.text = player.Balance + "ΔΜ";
        }
    }
}
