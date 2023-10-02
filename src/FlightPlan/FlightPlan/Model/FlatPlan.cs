using FlightPlan.Interface;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FlightPlan.Model
{
    public class FlatPlan : IFlatPlan
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

        public virtual string ToHeader(Dictionary<string, Destination> destDic, bool isFixedDestination)
        {
            if (isFixedDestination)
            {
                return $"DEST: {destination} - {destDic[destination].Name}";
            }
            else
            {
                return $"SOURCE: {source} - {destDic[source].Name}";
            }
        }

        public string ToName(Dictionary<string, Destination> destDic, bool isFixedDestination)
        {
            if (isFixedDestination)
            {
                return $"{source} - {destDic[source].Name}";
            }
            else
            {
                return $"{destination} - {destDic[destination].Name}";
            }
        }

        public string ToRichText()
        {
            return $"{departureTime}{((char)10).ToString()}{arrivalTime}";
        }

        public override string ToString()
        {
            return $"{source}-{destination} {day}/{month}/{year} {departureTime}:{arrivalTime}";
        }
    }
}
