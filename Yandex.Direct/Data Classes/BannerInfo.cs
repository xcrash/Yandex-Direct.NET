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
    }
}