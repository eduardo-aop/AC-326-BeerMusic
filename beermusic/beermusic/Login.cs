using System;

public class Login
{
    public string name { get; set; }
    public string ipAddress { get; set; }

    public Login(string json)
    {
        //jObject jObject = JObject.Parse(json);
        //_name = jObject["name"];
        //_ipAddress = (string)jObject["ip_address"];
    }
}
