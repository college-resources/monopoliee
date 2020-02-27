using UnityEngine;

public class AppLoader : MonoBehaviour
{
    #region Singleton
    public static AppLoader Instance { get; private set; }

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
        Session.Load();
    }
}
