using System;
using Schema;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{
    #region Singleton
    public static AuthenticationManager Instance { get; private set; }

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

    public User user;

    public async void Start()
    {
        try
        {
            var response = await ApiWrapper.AuthSession();
            user = User.GetUser(response["user"]);
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }

    public async UniTask LoginFormSubmit(string username, string password)
    {
        try
        {
            var response = await ApiWrapper.AuthLogin(username, password);
            user = User.GetUser(response["user"]);
            SceneManager.LoadScene("Home", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
            throw;
        }
    }

    public async UniTask RegisterFormSubmit(string username, string password)
    {
        try
        {
            var response = await ApiWrapper.AuthRegister(username, password);
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
            await ApiWrapper.AuthLogout();
            user = null;
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}
