using FlightPlan.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Interface
{
    public interface IFlatPlan
    {
        int day { get; set; }
        string ToHeader(Dictionary<string, Destination> destDic, bool isFixedDestination);

        string ToName(Dictionary<string, Destination> destDic, bool isFixedDestination);

        string ToRichText();
    }
}
