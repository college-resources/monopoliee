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
        Session.Instance.Subscribe(SessionListener);
        Game.Current.Subscribe(GameListener);
    }

    private static async void SessionListener(Session session)
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
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        else
        {
            switch (game.Status.Value)
            {
                case "waitingPlayers":
                    SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
                    break;
                case "running":
                    SceneManager.LoadScene("Game", LoadSceneMode.Single);
                    break;
                default:
                    Debug.Log("Unknown status: " + game.Status);
                    break;
            }
        }
    }
}
