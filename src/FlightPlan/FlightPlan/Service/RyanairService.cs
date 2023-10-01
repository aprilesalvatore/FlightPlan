using FlightPlan.Infrastructure;
using FlightPlan.Interface;
using FlightPlan.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightPlan.Service
{
    public class RyanairService : IRyanairService
    {
        private HttpService _httpService;
        public RyanairService(FlightPlanConfiguration configuration)
        {
            _httpService = new HttpService("https://www.ryanair.com");
        }

        public async Task<List<Destination>> GetDestinations()
        {
            var jtoken = await _httpService.Get<JToken>(_httpService.GetFullUrl($"booking/v4/it-it/res/stations"));

            var res = jtoken.Children().Select(x =>
             {
                 var val = x.First.ToString();

                 var destination = JsonConvert.DeserializeObject<Destination>(val);
                 destination.Tag = x.Path;
                 return destination;
             }).ToList(); ;

            return res;
        }

        public List<Tuple<int, int>> CalculateSheets(int? month, int year, bool exactlyMonth)
        {
            var res = new List<Tuple<int, int>>();

            if (exactlyMonth && month.HasValue)
            {
                res.Add(new Tuple<int, int>(month.Value, year));
            }
            else
            {
                int currentMonth = month.HasValue ? month.Value : DateTime.Now.Month;

                DateTime startDate = DateTime.Now;
                DateTime endDate = new DateTime(year, currentMonth, 1);

                DateTime iterator;
                DateTime limit;

                if (endDate > startDate)
                {
                    iterator = new DateTime(startDate.Year, startDate.Month, 1);
                    limit = endDate;
                }
                else
                {
                    iterator = new DateTime(endDate.Year, endDate.Month, 1);
                    limit = startDate;
                }

                while (iterator <= limit)
                {
                    res.Add(new Tuple<int, int>(iterator.Month, iterator.Year));

                    iterator = iterator.AddMonths(1);
                }
            }

            return res;
        }

        public async Task<Plan> GetFlightPlan(string source, string destination, int? month, int year)
        {
            int currentMonth = month.HasValue ? month.Value : DateTime.Now.Month;
            string url = $"timtbl/3/schedules/{source}/{destination}/years/{year}/months/{currentMonth}";
            var res = await _httpService.Get<Plan>(_httpService.GetFullUrl(url));

            if(res != null)
            {
                res.source = source;
                res.destination = destination;
            }
            return res;
        }


    }
}
