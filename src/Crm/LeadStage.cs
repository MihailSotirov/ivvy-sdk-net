using Ivvy.Json;
using Newtonsoft.Json;

namespace Ivvy.Crm
{
    /// <summary>
    /// An iVvy CRM lead stage.
    /// </summary>
    public class LeadStage : ISerializable
    {
        [JsonProperty("id")]
        public string Id
        {
            get; set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get; set;
        }
    }
}