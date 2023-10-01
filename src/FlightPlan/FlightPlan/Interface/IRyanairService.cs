using FlightPlan.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlan.Interface
{
    public interface IRyanairService
    {
        Task<List<Destination>> GetDestinations();

        List<Tuple<int, int>> CalculateSheets(int? month, int year, bool exactlyMonth);

        Task<Plan> GetFlightPlan(string source, string destination, int? month, int year);

        List<Multiplan> BuildMultiplan(Plan sourceStopPlan, Plan stopDestinationPlan);
    }
}
