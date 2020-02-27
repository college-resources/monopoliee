using System;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Login : MonoBehaviour
{
    public TextInput username;
    public TextInput password;
    public Submit submit;
    public Status statusMessage;

    private void Awake()
    {
        submit.Click += SubmitClick;
    }
    
    private async void SubmitClick(object sender, EventArgs args)
    {
        username.Highlight = false;
        password.Highlight = false;
        statusMessage.Value = "";

        try
        {
            await Session.LoginFormSubmit(username.Value, password.Value);
        }
        catch (BadResponseException e)
        {
            var response = e.Response;
            
            if (response["error"] != null)
            {
                statusMessage.Value = (string)response["error"]["message"];
            }
            else if (response["errors"] != null) 
            {
                var sb = new StringBuilder();
                var errors = (JArray) response["errors"];
                foreach (var err in errors.Children())
                {
                    if ((string)err["msg"] == "Invalid value")
                    {
                        switch ((string) err["param"])
                        {
                            case "username":
                                username.Highlight = true;
                                break;
                            case "password":
                                password.Highlight = true;
                                break;
                            default:
                                sb.AppendLine((string) err["msg"]);
                                break;
                        }
                    }
                }

                if (sb.Length != 0)
                {
                    statusMessage.Value = sb.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}
