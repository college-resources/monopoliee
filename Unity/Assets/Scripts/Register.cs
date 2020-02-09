using System;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Register : MonoBehaviour
{
    public TextInput username;
    public TextInput password;
    public TextInput confirmPassword;
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
        confirmPassword.Highlight = false;
        statusMessage.Value = "";

        if (password.Value == confirmPassword.Value)
        {
            try
            {
                await AuthenticationManager.Instance.RegisterFormSubmit(username.Value, password.Value);
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
        else
        {
            password.Highlight = true;
            confirmPassword.Highlight = true;
        }
    }
}
