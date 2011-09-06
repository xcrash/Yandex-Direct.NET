using Newtonsoft.Json;

namespace Yandex.Direct
{
    /// <summary>
    /// язык ответных сообщений от яндекса
    /// </summary>
    public enum YapiLanguage
    {
        ///<summary>јнглийский</summary>
        [JsonProperty("en")]
        English,

        ///<summary>–усский</summary>
        [JsonProperty("ru")]
        Russian,
        
        ///<summary>”краинский</summary>
        [JsonProperty("ua")]
        Ukrainian,
    }
}