using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leave : MonoBehaviour
{
    public SocketIo socketIo;
    
    public async void OnClick()
    {
        try
        {
            await ApiWrapper.GameLeave();
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
        
        SceneManager.LoadScene("Home", LoadSceneMode.Single);

        socketIo.Close();
    }
}
