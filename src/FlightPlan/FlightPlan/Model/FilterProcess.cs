using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightPlan.Model
{
    public class FilterProcess
    {
        public List<string> Source { get; set; }

        public List<string> Stop { get; set; }

        public List<string> Destination { get; set; }

        public bool IsCountryDestination { get; set; }

        public bool IsCountrySource { get; set; }

        public bool IsMultiplan { get; set; }

        public FilterProcess(string plan, List<Destination> destination)
        {
            var arr = plan.Split("-");

            if (arr.Length != 2 && arr.Length != 3)
                throw new InvalidOperationException("plan is not correct");

            var source = arr[0];
            var dest = arr[1];
            string stop = null;

            if (arr.Length == 3)
            {
                stop = arr[1];
                dest = arr[2];

                if (stop.Length != 3 || stop == "ALL")
                    throw new InvalidOperationException("stop is not correct in multi plan");
                else
                    Stop = new List<string>() { stop };

                if (source.Length != 3 || source == "ALL")
                    throw new InvalidOperationException("source is not correct in multi plan");
                else
                    Source = new List<string>() { source };

                IsMultiplan = true; 
            }
            else
            {
                if (source.Length == 2)
                {
                    Source = destination.Where(x => x.Country == source).Select(x => x.Tag).ToList();
                    IsCountrySource = true;
                }
                else if (source.ToUpperInvariant() == "ALL")
                {
                    Source = destination.Select(x => x.Tag).ToList();
                    IsCountrySource = true;
                }
                else
                    Source = new List<string>() { source };
            }

            if (dest.Length == 2)
            {
                Destination = destination.Where(x => x.Country == dest).Select(x => x.Tag).ToList();
                IsCountryDestination = true;
            }
            else if (dest.ToUpperInvariant() == "ALL")
            {
                Destination = destination.Select(x => x.Tag).ToList();
                IsCountryDestination = true;
            }
            else
                Destination = new List<string>() { dest };
        }
    }
}
