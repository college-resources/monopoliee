using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

using Schema;

public class NewGameDialog : MonoBehaviour
{
    public GameObject canvas;
    public GameObject create;
    public GameObject seats;

    private void Awake()
    {
        var canvasBtn = canvas.GetComponent<Button>();
        canvasBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        var seatsTxt = seats.GetComponent<TextInput>();
        var createBtn = create.GetComponent<Button>();
        createBtn.onClick.AddListener(async () =>
        {
            seatsTxt.Highlight = false;

            if (int.TryParse(seatsTxt.Value, out var seatsNumber) && seatsNumber > 0)
            {
                try
                {
                    var response = await ApiWrapper.GameNew(seatsNumber);
                    
                    var game = Game.GetGame(response);
                    GameManager.Instance.GoToLobby(game);
                }
                catch (BadResponseException e)
                {
                    var response = e.Response;
                    
                    if (response["error"] != null)
                    {
                        Debug.Log((string) response["error"]["message"]);
                    }
                    else if (response["errors"] != null)
                    {
                        var errors = (JArray) response["errors"];
                        foreach (var err in errors.Children())
                        {
                            if ((string) err["msg"] != "Invalid value") continue;
                                
                            if ((string) err["param"] == "seats")
                            {
                                seatsTxt.Highlight = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e); // TODO: Show error to player
                }
            }
            else
            {
                seatsTxt.Highlight = true;
            }
        });
    }
}
