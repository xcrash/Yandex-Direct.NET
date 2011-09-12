using System;
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

		[JsonConverter(typeof (StringEnumConverter))]
		public enum AutoBudgetPriorityType
		{
			[EnumMember(Value = "Low")] Low,
			[EnumMember(Value = "Medium")] Medium,
			[EnumMember(Value = "High")] High,
		}

		//imported from wsdl, untested
		public string IsRubric { get; set; }
		public int? Clicks { get; set; }
		public int? FirstPlaceClicks { get; set; }
		public int? PremiumClicks { get; set; }
		public int? Shows { get; set; }
		public float? Min { get; set; }
		public float? Max { get; set; }
		public float? PremiumMin { get; set; }
		public float? PremiumMax { get; set; }
		public float? CTR { get; set; }
		public float? FirstPlaceCTR { get; set; }
		public float? PremiumCTR { get; set; }
		public string LowCTRWarning { get; set; }
		public string LowCTR { get; set; }
		//public CoverageInfo[] ContextCoverage { get; set; }
		public float[] Prices { get; set; }
		//public CoverageInfo[] Coverage { get; set; }
		public string AutoBroker { get; set; }
		public float? CurrentOnSearch { get; set; }
		public float? MinPrice { get; set; }
		public string StatusPhraseModerate { get; set; }
		public string ContextLowCTR { get; set; }
		//public PhraseUserParams UserParams { get; set; }
	}
}