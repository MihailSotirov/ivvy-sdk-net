using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ivvy.API.Batch
{
    public class RunCompletedResponse
    {
        [JsonProperty("asyncId")]
        public string AsyncId
        {
            get; set;
        }

        [JsonProperty("results")]
        public List<JobResult> JobResults
        {
            get; set;
        }
    }
}