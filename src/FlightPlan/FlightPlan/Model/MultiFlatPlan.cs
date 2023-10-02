using FlightPlan.Interface;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FlightPlan.Model
{
    public class MultiFlatPlan : IFlatPlan
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

        public string ToHeader(Dictionary<string, Destination> destDic, bool isFixedDestination)
        {
            return $"SOURCE: [{source} - {destDic[source].Name}] STOP [{stop} - {destDic[stop].Name}] ";
        }

        public string ToName(Dictionary<string, Destination> destDic, bool isFixedDestination)
        {
            return $"{destination} - {destDic[destination].Name}";
        }

        public string ToRichText()
        {
            return $"{departureSourceTime}{((char)10).ToString()}{arrivalStopTime}{((char)10).ToString()}{departureStopTime}{((char)10).ToString()}{arrivalDestinationTime}";
        }

        public override string ToString()
        {
            return $"{source}-{stop}-{destination} {day}/{month}/{year} {departureSourceTime}:{arrivalStopTime} - {departureStopTime}:{arrivalDestinationTime}";
        }
    }
}
