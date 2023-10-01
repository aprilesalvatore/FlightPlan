﻿using FlightPlan.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Interface
{
    public interface IExcelService
    {
        ExcelPackage Create(string filename);

        void CreateSheet(ExcelPackage package, List<Plan> plans, int year, int month, List<Destination> destinations, bool isFixedDestination);

        void CreateSheetMultiplan(ExcelPackage package, List<Multiplan> plans, int month, int year, List<Destination> destinations, bool isFixedDestination);
    }
}
