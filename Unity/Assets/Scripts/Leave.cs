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
            await APIWrapper.Instance.GameLeave();
            
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }

        socketIo.Close();
    }
}
