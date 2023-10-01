using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Model
{
    public class FlatPlan
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public string carrierCode { get; set; }
        public string number { get; set; }
        public string departureTime { get; set; }
        public string arrivalTime { get; set; }
        public string destination { get; set; }
        public string source { get; set; }

        public override string ToString()
        {
            return $"{source}-{destination} {day}/{month}/{year} {departureTime}:{arrivalTime}";
        }
    }
}
