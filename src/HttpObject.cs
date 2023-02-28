using System.Text.Json.Serialization;
using OpenDatabase.Json;

namespace CourseDB.WebAPI;
using Newtonsoft.Json;

public enum HttpReturnType
{
    Error,
    Success
}

public class HttpObject
{
    public HttpReturnType Type;

    public object Payload;

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public HttpObject(HttpReturnType type, object payload)
    {
        this.Type = type;
        this.Payload = payload;
    }
}


