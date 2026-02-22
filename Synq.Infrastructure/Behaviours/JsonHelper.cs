using System.Text;
using System.Text.Json;
using Synq.Application.Common.Interfaces;

namespace Synq.Infrastructure.Behaviours;

public class JsonHelper<ObjectType> : IJsonHelper<ObjectType>
{
    public ObjectType? Decode(string encoded)
    {
        byte[] bytes = Convert.FromBase64String(encoded);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<ObjectType>(json);
    }

    public string Encode(ObjectType data)
    {
        var json = JsonSerializer.Serialize(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }
}