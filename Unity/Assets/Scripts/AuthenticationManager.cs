using System;
using System.Threading.Tasks;
using Schema;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{
    #region Singleton
    public static AuthenticationManager Instance { get => _instance; }

    private static AuthenticationManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public User user;

    public async void Start()
    {
        try
        {
            var response = await APIWrapper.Instance.AuthSession();
            user = User.GetUser(response["user"]);
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }

    public async Task LoginFormSubmit(string username, string password)
    {
        try
        {
            var response = await APIWrapper.Instance.AuthLogin(username, password);
            user = User.GetUser(response["user"]);
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
            throw;
        }
    }

    public async Task RegisterFormSubmit(string username, string password)
    {
        try
        {
            var response = await APIWrapper.Instance.AuthRegister(username, password);
            user = User.GetUser(response["user"]);
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
            throw;
        }
    }

    public async void Logout()
    {
        try
        {
            await APIWrapper.Instance.AuthLogout();
            user = null;
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}
