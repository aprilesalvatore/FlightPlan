using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Model
{
    public class Destination
    {
        public string Tag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        public override string ToString()
        {
            return $"TAG: {Tag} Country: {Country} Name: {Name}";
        }
    }
}
