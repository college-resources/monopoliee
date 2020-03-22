// #define MONOPOLIEE_PRODUCTION_MODE

using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiWrapper
{
    #if MONOPOLIEE_PRODUCTION_MODE
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "monopoliee.cores.gr/";
    public const string COOKIE_NAME = "connect.sid";
    #else
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "localhost:3000/";
    public const string COOKIE_NAME = "connect.sid";
    #endif
    
    public static BehaviorSubject<string> Cookie { get; }

    static ApiWrapper()
    {
        var req = new UnityWebRequest(HTTP_PROTOCOL + URL);
        var cookie = req.GetRequestHeader("Cookie");
        Cookie = new BehaviorSubject<string>(string.IsNullOrEmpty(cookie) ? "" : cookie);
        Cookie.Subscribe(Debug.Log);
    }
    
    public static async UniTask<JToken> AuthLogin(string username, string password)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        return await Upload("auth/login", form);
    }

    public static async UniTask<JToken> AuthLogout()
    {
        return await Upload("auth/logout");
    }
    
    public static async UniTask<JToken> AuthRegister(string username, string password)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        return await Upload("auth/register", form);
    }

    public static async UniTask<JToken> AuthSession()
    {
        return await Upload("auth/session");
    }

    public static async UniTask<JToken> GameNew(int seats, bool inviteOnly = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("seats", seats);
        if (inviteOnly)
        {
            form.AddField("invite_only", "true");
        }

        return await Upload("game/new", form);
    }

    public static async UniTask<JToken> GameJoin(string gameId, string invitationCode = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", gameId);
        if (!string.IsNullOrEmpty(invitationCode))
        {
            form.AddField("invitation_code", invitationCode);
        }

        return await Upload("game/join", form);
    }

    public static async UniTask<JToken> GameList()
    {
        return await Upload("game/list");
    }

    public static async UniTask<JToken> GameCurrent()
    {
        return await Upload("game/current");
    }
    
    public static async UniTask<JToken> GameLeave()
    {
        return await Upload("game/leave");
    }
    
    public static async UniTask<JToken> GamePrices()
    {
        return await Upload("game/prices");
    }
    
    public static async UniTask<JToken> PlayerRollDice()
    {
        return await Upload("player/roll-dice");
    }
    
    public static async UniTask<JToken> PlayerEndTurn()
    {
        return await Upload("player/end-turn");
    }
    
    public static async UniTask<JToken> TransactionBuyCurrentProperty()
    {
        return await Upload("transaction/buy-current-property");
    }

    public static async UniTask<JToken> SocketIo()
    {
        var www = UnityWebRequest.Get(HTTP_PROTOCOL + URL + "socket.io/?EIO=3&transport=polling");
        
        var resText = await GetTextAsync(www);
        var json = resText.Split(new[] {":0"}, StringSplitOptions.None)[1];
        var response = JToken.Parse(json);
        
        if (www.error != null)
        {
            throw new BadResponseException(www.error, response);
        }
        
        return response;
    }

    private static async UniTask<string> GetTextAsync(UnityWebRequest req)
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        req.SetRequestHeader("Cookie", Cookie.Value);
        #endif
        
        var op = await req.SendWebRequest();
        if (op.error == "Cannot connect to destination host")
        {
            throw new IOException(op.error);
        }

        #if !UNITY_WEBGL || UNITY_EDITOR
        var responseHeaders = req.GetResponseHeaders();
        if (responseHeaders.ContainsKey("Set-Cookie") && responseHeaders["Set-Cookie"].StartsWith(COOKIE_NAME))
        {
            Cookie.OnNext(responseHeaders["Set-Cookie"]);
        }
        #endif
        
        return op.downloadHandler.text;
    }
    
    private static async UniTask<JToken> Upload(string path, WWWForm form = null)
    {
        var www = 
            form == null 
                ? UnityWebRequest.Get(HTTP_PROTOCOL + URL + path)
                : UnityWebRequest.Post(HTTP_PROTOCOL + URL + path, form);
        
        var resText = await GetTextAsync(www);
        var response = JToken.Parse(resText);

        if (www.error != null)
        {
            throw new BadResponseException(www.error, response);
        }
        
        return response;
    }
}
