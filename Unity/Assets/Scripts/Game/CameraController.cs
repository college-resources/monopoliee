using System.Collections.Generic;
using Schema;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Game _game;
    public List<GameObject> cameraList;
    private CurrentGame _currentGame;

    private void Start()
    {
        _currentGame = GameObject.Find("CurrentGame").GetComponent<CurrentGame>();
        _game = Game.Current.Value;
    }

    public void SetUpCameras()
    {
        foreach (var player in _currentGame.playerList)
        {
            cameraList.Add(player.transform.Find("VirtualCamera").gameObject);
        }
        
        foreach (var vCamera in cameraList)
        {
            vCamera.gameObject.SetActive(false);
        }

        if (cameraList.Count > 0)
        {
            var currentPlayerId = _game.CurrentPlayerId.Value;
            var index = Player.GetPlayerById(currentPlayerId).Index;
            cameraList[index].gameObject.SetActive(true);
        }
    }

    public void FocusCameraOn(Player player)
    {
        for (var i = 0; i < cameraList.Count; i++)
        {
            cameraList[i].gameObject.SetActive(i == player.Index);
        }
    }
}
