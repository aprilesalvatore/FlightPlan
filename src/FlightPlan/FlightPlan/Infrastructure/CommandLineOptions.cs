using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Infrastructure
{
    public class CommandLineOptions
    {
        [Option('p', "plan", Required = false, HelpText = "Specify the source and destination: several example: STN-IT (Source: London Stansted - Destination: Any part of italy) IT-STN (Source: Any part of italy - Destination: London Stansted) STN-BVA (Source: London Stansted - Destination: Paris Beauvais) IT-FR (Source: Any Part of Italy- Destination: Any Part of France) BRI-ALL (Source: Bari - Destination: All ryanair destination)")]
        public string Plan { get; set; }

        [Option('c', "country", Required = false, HelpText = "Print a list of available country")]
        public bool PrintCountry { get; set; }

        [Option('d', "destination", Required = false, HelpText = "Print a list of available destination")]
        public bool PrintDestination { get; set; }

        [Option('m', "mount", Required = false, HelpText = "Specify the numer of mounth")]
        public int? Mounth { get; set; }

        [Option('e', "exact", Required = false, HelpText = "If this parameter is added, we consider the exactly month")]
        public bool ExactlyMonth { get; set; }

        [Option('y', "year", Required = false, HelpText = "Specify the numer of year (Eg. 2023)")]
        public int? Year { get; set; }

    }
}
