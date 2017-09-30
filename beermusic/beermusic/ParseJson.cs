using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ParseJson
{
    public static Login parseLoginJson(String json)
    {
        Login login = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(json);
        return login;
    }
}
