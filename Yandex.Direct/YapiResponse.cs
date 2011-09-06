using Newtonsoft.Json;

namespace Yandex.Direct
{
    [JsonObject]
    public class YapiResponse<T>
    {
        public T Data;
    }
}