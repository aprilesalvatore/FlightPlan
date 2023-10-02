using FlightPlan.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Model
{
    public class Day
    {
        public int day { get; set; }
        public List<Flight> flights { get; set; }
    }

    public class Flight
    {
        public string carrierCode { get; set; }
        public string number { get; set; }
        public string departureTime { get; set; }
        public string arrivalTime { get; set; }

        public string destination { get; set; }
        public string source { get; set; }
    }

    public class Plan: IPlan
    {
        public string destination { get; set; }
        public string source { get; set; }
        public int month { get; set; }
        public List<Day> days { get; set; }  
    }
}
