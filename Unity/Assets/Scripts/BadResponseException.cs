using System;
using Newtonsoft.Json.Linq;

public class BadResponseException : Exception
{
    public JToken Response { get; }

    public BadResponseException(string message, JToken response) : base(message)
    {
        Response = response;
    }
}