using System;
using Schema;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneListener : MonoBehaviour
{
    #region Singleton
    private static SceneListener Instance { get; set; }

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

    private void Start()
    {
        Session.Instance.Subscribe(SessionSceneListener);
    }

    private static async void SessionSceneListener(Session session)
    {
        if (session == null)
        {
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
        else
        {
            try
            {
                var response = await ApiWrapper.GameCurrent();
            
                Game.ClearCache();
                var gameToJoin = Game.GetGame(response);

                switch (gameToJoin.Status.Value)
                {
                    case "waitingPlayers":
                        GameManager.Instance.GoToLobby(gameToJoin);
                        break;
                    case "running":
                        GameManager.Instance.GoToGame(gameToJoin);
                        break;
                    default:
                        Debug.Log("Unknown status: " + gameToJoin.Status);
                        break;
                }
            }
            catch (BadResponseException)
            {
                SceneManager.LoadScene("Home", LoadSceneMode.Single);
            }
            catch (Exception e)
            {
                Debug.Log(e); // TODO: Show error to player
            }
        }
    }
}
