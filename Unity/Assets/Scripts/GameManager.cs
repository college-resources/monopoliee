using UnityEngine;
using UnityEngine.SceneManagement;
using Schema;

public class GameManager : MonoBehaviour
{
    #region Singleton
    
    public static GameManager Instance { get; private set; }

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

    public Game Game { get; private set; }

    public void GoToGame(Game game)
    {
        Game = game;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    
    public void GoToLobby(Game game)
    {
        Game = game;
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
