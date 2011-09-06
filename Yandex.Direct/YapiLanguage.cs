using Newtonsoft.Json;

namespace Yandex.Direct
{
    /// <summary>
    /// ���� �������� ��������� �� �������
    /// </summary>
    public enum YapiLanguage
    {
        ///<summary>����������</summary>
        [JsonProperty("en")]
        English,

        ///<summary>�������</summary>
        [JsonProperty("ru")]
        Russian,
        
        ///<summary>����������</summary>
        [JsonProperty("ua")]
        Ukrainian,
    }
}