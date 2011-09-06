using System;
using System.Diagnostics;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public AutoBudgetPriorityType? AutoBudgetPriority
        {
            get { return _autoBudgetPriority == null ? (AutoBudgetPriorityType?)null : (AutoBudgetPriorityType)Enum.Parse(typeof(AutoBudgetPriorityType), _autoBudgetPriority); }
            set { _autoBudgetPriority = value == null ? null : value.ToString(); }
        }
        [JsonProperty("AutoBudgetPriority"), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _autoBudgetPriority;

        public enum AutoBudgetPriorityType
        {
            Low,
            Medium,
            High,
        }
    }
}