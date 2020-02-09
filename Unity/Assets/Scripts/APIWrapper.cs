// #define MONOPOLIEE_PRODUCTION_MODE

using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class APIWrapper : MonoBehaviour
{
    #region Singleton
    public static APIWrapper Instance { get; private set; }

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
            
            _queue = new CoroutineQueue(this);
            _queue.StartLoop();
        }
    }
    #endregion

    #if MONOPOLIEE_PRODUCTION_MODE
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "monopoliee.cores.gr/";
    #else
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "localhost:3000/";
    #endif

    private CoroutineQueue _queue;

    public delegate void APICallback(JToken response, string error = null);

    public Task<JToken> AuthLogin(string username, string password)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        return Upload("auth/login", form);
    }

    public Task<JToken> AuthLogout()
    {
        return Upload("auth/logout", null);
    }
    
    public Task<JToken> AuthRegister(string username, string password)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        return Upload("auth/register", form);
    }

    public Task<JToken> AuthSession()
    {
        return Upload("auth/session", null);
    }

    public Task<JToken> GameNew(int seats, bool inviteOnly = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("seats", seats);
        if (inviteOnly)
        {
            form.AddField("invite_only", "true");
        }

        return Upload("game/new", form);
    }

    public Task<JToken> GameJoin(string gameId, string invitationCode = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", gameId);
        if (!string.IsNullOrEmpty(invitationCode))
        {
            form.AddField("invitation_code", invitationCode);
        }

        return Upload("game/join", form);
    }

    public Task<JToken> GameList()
    {
        return Upload("game/list", null);
    }

    public Task<JToken> GameCurrent()
    {
        return Upload("game/current", null);
    }
    
    public Task<JToken> GameLeave()
    {
        return Upload("game/leave", null);
    }
    
    public Task<JToken> GamePrices()
    {
        return Upload("game/prices", null);
    }
    
    public Task<JToken> PlayerRollDice()
    {
        return Upload("player/roll-dice", null);
    }
    
    public Task<JToken> PlayerEndTurn()
    {
        return Upload("player/end-turn", null);
    }
    
    public Task<JToken> TransactionBuyCurrentProperty()
    {
        return Upload("transaction/buy-current-property", null);
    }

    private Task<JToken> Upload(string path, WWWForm form = null)
    {
        var t = new TaskCompletionSource<JToken>();
        
        _queue.EnqueueAction(Upload(path, form, (response, error) =>
        {
            if (error != null && response != null)
            {
                t.TrySetException(new BadResponseException(error, response));
            }
            else if (error != null)
            {
                t.TrySetException(new Exception(error));
            }
            else
            {
                t.TrySetResult(response);
            }
        }));
        
        return Task.Run(() => t.Task);
    }
    
    private static IEnumerator Upload(string path, WWWForm form = null, APICallback callback = null)
    {
        var www = 
            form == null 
                ? UnityWebRequest.Get(HTTP_PROTOCOL + URL + path)
                : UnityWebRequest.Post(HTTP_PROTOCOL + URL + path, form);
        yield return www.SendWebRequest();

        if (callback != null)
        {
            try
            {
                var resText = www.downloadHandler.text;
                var response = JToken.Parse(resText);
                callback(response, www.error);
            }
            catch (JsonException ex)
            {
                callback(null, ex.Message);
            }
        }
    }
}
