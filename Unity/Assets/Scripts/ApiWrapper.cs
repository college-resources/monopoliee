#define MONOPOLIEE_PRODUCTION_MODE

using Newtonsoft.Json.Linq;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiWrapper
{
    #if MONOPOLIEE_PRODUCTION_MODE
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "monopoliee.cores.gr/";
    #else
    public const string HTTP_PROTOCOL = "http://";
    public const string WS_PROTOCOL = "ws://";
    public const string URL = "localhost:3000/";
    #endif
    
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

    private static async UniTask<string> GetTextAsync(UnityWebRequest req)
    {
        var op = await req.SendWebRequest();
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
