using System;
using Newtonsoft.Json.Linq;

public class BadResponseException : Exception
{
    private JToken _response;

    public JToken Response => _response;

    public BadResponseException(string message, JToken response) : base(message)
    {
        _response = response;
    }
}