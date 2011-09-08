using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Yandex.Direct
{
    [JsonObject, DebuggerDisplay("{Id}: {Name}")]
    public class ShortCampaignInfo
    {
        [JsonProperty("CampaignID")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public DateTime StartDate { get; set; }

        public string AgencyName { get; set; }
        public string ManagerName { get; set; }

        public decimal Sum { get; set; }
        public decimal Rest { get; set; }

        public int Shows { get; set; }
        public int Clicks { get; set; }

        public string Status { get; set; }
        public string StatusShow { get; set; }
        public string StatusModerate { get; set; }
        public string StatusActivating { get; set; }
        public string StatusArchive { get; set; }

        // ReSharper disable UnassignedField.Local
        [JsonIgnore]
        public bool IsActive
        {
            get { return _isActive == "Yes"; }
        }
        [JsonProperty("IsActive"), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _isActive;
        // ReSharper restore UnassignedField.Local

        public decimal SumAvailableForTransfer { get; set; }
    }
}