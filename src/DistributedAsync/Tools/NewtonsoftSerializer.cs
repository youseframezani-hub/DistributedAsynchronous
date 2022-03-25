using DistributedAsync.Abstractions;
using Newtonsoft.Json;

namespace DistributedAsync.Tools
{
    class NewtonsoftSerializer : ISerializer
    {
        public T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text);
        public string Serialize(object value) => JsonConvert.SerializeObject(value);
    }
}
