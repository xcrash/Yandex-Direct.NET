using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Yandex.Direct
{
    [JsonObject]
    public class BannerPhraseInfo
    {
        [JsonProperty("BannerID")]
        public int BannerId { get; set; }

        [JsonProperty("CampaignID")]
        public int CampaignId { get; set; }

        [JsonProperty("PhraseID")]
        public int PhraseId { get; set; }

        public string Phrase { get; set; }

        public decimal? Price { get; set; }
        public decimal? ContextPrice { get; set; }

        public AutoBudgetPriorityType? AutoBudgetPriority { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum AutoBudgetPriorityType
        {
            [EnumMember(Value = "Low")]
            Low,
            [EnumMember(Value = "Medium")]
            Medium,
            [EnumMember(Value = "High")]
            High,
        }
    }
}