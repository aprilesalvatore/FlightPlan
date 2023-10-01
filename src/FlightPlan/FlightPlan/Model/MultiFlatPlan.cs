using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Model
{
    public class MultiFlatPlan
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public string departureSourceTime { get; set; }
        public string arrivalStopTime { get; set; }

        public string departureStopTime { get; set; }

        public string arrivalDestinationTime { get; set; }
        public string destination { get; set; }
        public string source { get; set; }
        public string stop { get; set; }

        public override string ToString()
        {
            return $"{source}-{stop}-{destination} {day}/{month}/{year} {departureSourceTime}:{arrivalStopTime} - {departureStopTime}:{arrivalDestinationTime}";
        }
    }
}
