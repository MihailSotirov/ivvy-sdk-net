using System;
using Ivvy.API.Common;
using Ivvy.API.Json;
using Newtonsoft.Json;

namespace Ivvy.API.Venue
{
    /// <summary>
    /// A guest can be assigned to an accommodation room reservation on an iVvy venue booking.
    /// </summary>
    public class Guest : ISerializable
    {
        [JsonProperty("id")]
        public int? Id
        {
            get; set;
        }

        [JsonProperty("contact")]
        public GuestContact Contact
        {
            get; set;
        }

        [JsonProperty("primaryPhone")]
        public string PrimaryPhone
        {
            get; set;
        }

        [JsonProperty("address")]
        public Address Address
        {
            get; set;
        }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate
        {
            get; set;
        }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate
        {
            get; set;
        }
    }
}