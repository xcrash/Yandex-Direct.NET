using System.Diagnostics;
using Newtonsoft.Json;

namespace Yandex.Direct
{
    [JsonObject, DebuggerDisplay("{Login} - {UnitsRest} unit(s)")]
    public class ClientUnitInfo
    {
        public string Login { get; set; }
        public int UnitsRest { get; set; }
    }
}