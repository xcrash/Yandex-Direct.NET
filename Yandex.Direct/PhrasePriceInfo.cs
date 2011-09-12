namespace Yandex.Direct
{
	public class PhrasePriceInfo
	{
		public int PhraseID { get; set; }

		public int? BannerID { get; set; }
		
		public int? CampaignID { get; set; }

		public float? Price { get; set; }

		public string AutoBroker { get; set; }

		public string AutoBudgetPriority { get; set; }

		public float? ContextPrice { get; set; }
	}
}