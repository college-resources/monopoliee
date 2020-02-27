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
        Session.Instance.Skip(1).Subscribe(SessionListener);
        Game.Current.Skip(1).Subscribe(GameListener);
    }

    private static async void SessionListener(Session session)
    {
        if (session == null)
        {
            ChangeScene("Login");
        }
        else
        {
            try
            {
                var response = await ApiWrapper.GameCurrent();
                Game.Join(response);
            }
            catch (BadResponseException)
            {
                Game.Current.OnNext(null);
            }
            catch (Exception e)
            {
                Debug.Log(e); // TODO: Show error to player
            }
        }
    }

    private static void GameListener(Game game)
    {
        if (game == null)
        {
            ChangeScene("Home");
        }
        else
        {
            game.Status.Subscribe(GameStatusListener);
        }
    }

    private static void GameStatusListener(string status)
    {
        switch (status)
        {
            case "waitingPlayers":
            case "starting":
                ChangeScene("Lobby");
                break;
            case "running":
                ChangeScene("Game");
                break;
            default:
                Debug.Log($"Unknown status: {status}");
                break;
        }
    }
    
    private static void ChangeScene(string scene)
    {
        if (SceneManager.GetActiveScene().name != scene)
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        
        Debug.Log($"Current scene: {scene}");
    }
}
