using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yandex.Direct
{
	[JsonObject]
	public class BannerInfo
	{
		[JsonProperty("BannerID")]
		public int BannerId { get; set; }

		[JsonProperty("CampaignID")]
		public int CampaignId { get; set; }

		public string Title { get; set; }
		public string Text { get; set; }
		public string Href { get; set; }
		public string Geo { get; set; }

		public BannerPhraseInfo[] Phrases { get; set; }

		[JsonProperty("Sitelinks")]
		public List<SiteLinkInfo> SiteLinks { get; set; }
		public List<string> MinusKeywords { get; set; }

		//imported from wsdl, untested
		public string Domain { get; set; }
		//public ContactInfo ContactInfo { get; set; }
		public string StatusActivating { get; set; }
		public string StatusArchive { get; set; }
		public string StatusBannerModerate { get; set; }
		public string StatusPhrasesModerate { get; set; }
		public string StatusPhoneModerate { get; set; }
		public string StatusShow { get; set; }
		public string IsActive { get; set; }
		public string StatusSitelinksModerate { get; set; }
		//public object[] AdWarnings { get; set;}
		public string FixedOnModeration { get; set; }
		//public RejectReason[] ModerateRejectionReasons { get; set;}
	}
}