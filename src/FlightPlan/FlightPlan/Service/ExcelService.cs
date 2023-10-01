using FlightPlan.Interface;
using FlightPlan.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlightPlan.Service
{
    public class ExcelService : IExcelService
    {
        public ExcelPackage Create(string filename)
        {
            string my_folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $".\\Excel";

            if (!Directory.Exists(my_folder))
                Directory.CreateDirectory(my_folder);

            return new ExcelPackage($"{my_folder}\\{filename}_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.xlsx");
        }

        public void CreateSheet(ExcelPackage package, List<Plan> plans, int month, int year, List<Destination> destinations, bool isFixedDestination)
        {
            var sheetName = $"{month}-{year}";

            var sheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName);
            if (sheet == null)
                sheet = package.Workbook.Worksheets.Add(sheetName);

            var flatPlan = BuildFlatPlan(plans, year, month);

            flatPlan.ForEach(x => Debug.WriteLine(x));

            var destDic = destinations.ToDictionary(x => x.Tag, y => y);

            if (flatPlan != null && flatPlan.Count > 0)
            {
                var grouped = flatPlan.GroupBy(x => x.day).OrderBy(x => x.Key).ToList();

                foreach (var item in grouped)
                {
                    var index = grouped.IndexOf(item) + 2;

                    sheet.Cells[1, 1].Value = CalculateNameHeader(isFixedDestination, destDic, item);
                    sheet.Cells[1, index].Value = $"{item.Key} {new DateTime(year, month, item.Key).ToString("ddd")}";
                    SetStyle(sheet.Cells[1, 1], true);
                    SetStyle(sheet.Cells[1, index], true);

                    sheet.Row(index).Height = 30;
                }

                var cls = new List<Header>();

                var dest = flatPlan.Select(x => CalculateName(isFixedDestination, destDic, x)).Distinct().ToList();

                dest.ForEach(x =>
                {
                    foreach (var item in grouped)
                    {
                        var head = new Header() { Count = item.Count(y => CalculateName(isFixedDestination, destDic, y) == x), Name = x };

                        cls.Add(head);
                    }
                });

                var orderdedHeader = cls.OrderBy(x => x.Name).GroupBy(x => x.Name).Select(x => new Header()
                {
                    Name = x.Key,
                    Count = x.Max(u => u.Count)
                }).ToList();

                var lst = new List<string>();

                foreach (var item in orderdedHeader)
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        lst.Add(item.Name);
                    }
                }

                for (int i = 0; i < lst.Count; i++)
                {
                    var item = lst[i];

                    var index = i + 2;

                    sheet.Cells[index, 1].Value = item;
                    SetStyle(sheet.Cells[index, 1], false);

                    sheet.Row(index).Height = 30;
                }

                int rowNumber = lst.Count;

                foreach (var item in grouped)
                {
                    var flt = item.ToList();
                    var col = grouped.IndexOf(item) + 2;

                    foreach (var plan in flt)
                    {
                        var name = CalculateName(isFixedDestination, destDic, plan);

                        for (int rowCol = 2; rowCol < rowNumber + 2; rowCol++)
                        {
                            if (name == sheet.Cells[rowCol, 1].Value.ToString() && sheet.Cells[rowCol, col].Value == null)
                            {
                                sheet.Cells[rowCol, col].RichText.Add($"{plan.departureTime}{((char)10).ToString()}{plan.arrivalTime}");
                                sheet.Cells[rowCol, col].Style.WrapText = true;
                                sheet.Row(rowCol).Height = 30;
                                SetStyle(sheet.Cells[rowCol, col], false);
                                break;
                            }

                            SetStyle(sheet.Cells[rowCol, col], false);
                        }
                    }
                }

                sheet.Columns.ToList().ForEach(x =>
                {
                    if (x.StartColumn == 1)
                        x.AutoFit();
                    else
                        x.Width = 7.5;
                });
            }
        }

        private string CalculateNameHeader(bool isFixedDestination, Dictionary<string, Destination> destDic, IGrouping<int, FlatPlan> item)
        {
            if (isFixedDestination)
            {
                return $"DEST: {item.FirstOrDefault().destination} - {destDic[item.FirstOrDefault().destination].Name}";
            }
            else
            {
                return $"SOURCE: {item.FirstOrDefault().source} - {destDic[item.FirstOrDefault().source].Name}";
            }
        }

        private string CalculateName(bool isFixedDestination, Dictionary<string, Destination> destDic, FlatPlan plan)
        {
            if (isFixedDestination)
            {
                return $"{plan.source} - {destDic[plan.source].Name}";
            }
            else
            {
                return $"{plan.destination} - {destDic[plan.destination].Name}";
            }
        }

        private void SetStyle(ExcelRange range, bool isHeader)
        {
            if (isHeader)
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
            }

            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;

        }

        private List<FlatPlan> BuildFlatPlan(List<Plan> plan, int year, int month)
        {
            var flatted = plan.SelectMany(x => x.days.SelectMany(y => y.flights.Select(z => new FlatPlan()
            {
                arrivalTime = z.arrivalTime,
                carrierCode = z.carrierCode,
                departureTime = z.departureTime,
                number = z.number,
                day = y.day,
                month = month,
                year = year,
                destination = x.destination,
                source = x.source
            }))).ToList();

            return flatted;
        }

    }
}
