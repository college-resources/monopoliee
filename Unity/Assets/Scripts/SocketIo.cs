using System;
using System.Collections;
using System.Text;
using System.Threading;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Schema;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnPlayerJoined(Player player);

public class SocketIo : MonoBehaviour
{
    private WebSocket _websocket;
    public event OnPlayerJoined PlayerJoined;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Upload("socket.io/?EIO=3&transport=polling", null, async (response, error) =>
        {
            if (error != null) return;
            
            string sid = (string) response["sid"];

            _websocket = new WebSocket("ws://localhost:3000/socket.io/?EIO=3&transport=websocket&sid=" + sid);

            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
                _websocket.SendText("2probe");
            };

            _websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
                StartCoroutine(waiter());
            };

            _websocket.OnMessage += (bytes) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (message == "3probe")
                    {
                        _websocket.SendText("5");
                        return;
                    }

                    if (message.Length > 3 && message.Substring(0, 2) == "42")
                    {
                        message = message.Substring(2);
                        JArray array = JArray.Parse(message);
            
                        if ((string) array[0] == "playerJoined")
                        {
                            Player player = Player.GetPlayer(array[1]["player"]);
                            PlayerJoined?.Invoke(player);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            };

            // Keep sending messages every 15s
            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 5.0f);
            
            // Waiting for messages
            await _websocket.Connect();
        }));
    }
    
    IEnumerator waiter()
    {
        yield return new WaitForSecondsRealtime(5);
        Start();
    }

    async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _websocket.SendText("2");
        }
    }

    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }
    
    private IEnumerator Upload(string path, WWWForm form = null, APIWrapper.APICallback callback = null)
    {
        // TODO: Hardcoded URL
        using (UnityWebRequest www = 
            form == null 
                ? UnityWebRequest.Get("http://localhost:3000/" + path)
                : UnityWebRequest.Post("http://localhost:3000/" + path, form))
        {
            yield return www.SendWebRequest();

            if (callback != null)
            {
                try
                {
                    string resText = www.downloadHandler.text;
                    string json = resText.Split(new[] {":0"}, StringSplitOptions.None)[1];
                    JToken response = JToken.Parse(json);
                    callback(response, www.error);
                }
                catch (JsonException ex)
                {
                    callback(null, ex.Message);
                }
            }
        }
    }
}
