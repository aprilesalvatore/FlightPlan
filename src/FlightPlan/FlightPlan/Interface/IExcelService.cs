using FlightPlan.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Interface
{
    public interface IExcelService
    {
        ExcelPackage Create(string filename);

        void CreateSheet(ExcelPackage package, List<IPlan> plans, int year, int month, List<Destination> destinations, bool isFixedDestination);
    }
}
