using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FlightPlan.Infrastructure;
using FlightPlan.Service;
using FlightPlan.Interface;
using OfficeOpenXml;
using FlightPlan.Model;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;

namespace FlightPlan
{
    public class RunnerContext
    {
        private IRyanairService _ryanairService;
        private IConfiguration _configuration;
        private IContainer _container;
        private FlightPlanConfiguration _flightPlanConfiguration;
        public RunnerContext()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            Build(configuration);
        }

        public RunnerContext(IConfiguration configuration)
        {
            Build(configuration);
        }


        private void Build(IConfiguration configuration)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;

            _flightPlanConfiguration = _configuration.GetSection(nameof(FlightPlanConfiguration)).Get<FlightPlanConfiguration>();

            var provider = BuildServices(_flightPlanConfiguration, _configuration);
        }

        private IServiceProvider BuildServices(FlightPlanConfiguration flightConfig, IConfiguration config)
        {
            var serviceCollection = new ServiceCollection();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            containerBuilder.RegisterType<RyanairService>().As<IRyanairService>()
                .WithParameter("configuration", _flightPlanConfiguration)
                .SingleInstance();

            containerBuilder.RegisterType<ExcelService>().As<IExcelService>()
                .SingleInstance();

            _container = containerBuilder.Build();
            return new AutofacServiceProvider(_container);
        }

        public void Run(string[] args)
        {
            try
            {
                var cts = new CancellationTokenSource();
                Console.WriteLine($"Flight Plan Running - Press Ctrl + C to Finish");

                Console.CancelKeyPress += (s, e) =>
                {
                    Console.WriteLine($"Flight Plan Canceling...");
                    cts.Cancel();
                    e.Cancel = true;
                };
                _ryanairService = _container.Resolve<IRyanairService>();
                InternalRun(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Flight Plan unspected exception. message:{ex.Message}, exception: {ex.ToExceptionString()}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        private void InternalRun(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(option =>
            {

                Console.WriteLine($".... Get Destinations");
                var destinantions = Task.Run(() => _ryanairService.GetDestinations()).Result;

                Console.WriteLine($".... Found {destinantions.Count} destinations");

                if (option.PrintDestination && destinantions.Count > 0)
                    destinantions.ForEach(x => Console.WriteLine(x));
                else if (option.PrintCountry && destinantions.Count > 0)
                    destinantions.Select(x => x.Country).Distinct().ToList().ForEach(x => Console.WriteLine($"Country: {x}"));
                else if (!String.IsNullOrEmpty(option.Plan))
                {
                    var plans = new List<Plan>();
                    if (!option.Year.HasValue)
                        throw new ArgumentNullException("Year");

                    var excelService = _container.Resolve<IExcelService>();
                    Console.WriteLine($".... RyanAir flight plan processing.");

                    var sheets = _ryanairService.CalculateSheets(option.Mounth, option.Year.Value, option.ExactlyMonth);
                    var filter = new FilterProcess(option.Plan, destinantions);

                    Console.WriteLine($".... Is Multiplan: {filter.IsMultiplan}.");

                    using (var excel = excelService.Create("RyanAir"))
                    {
                        foreach (var sheet in sheets)
                        {
                            foreach (var source in filter.Source)
                            {
                                if (filter.IsMultiplan)
                                    plans = RunMultiple(filter, sheet, source);
                                else
                                    plans = RunSingle(filter, sheet, source);
                            }

                            if (plans.Count > 0)
                            {
                                var lst = plans.OfType<IPlan>().ToList();
                                int height = 30;

                                if (filter.IsMultiplan)
                                {
                                    height = 60;
                                    var res = new List<Multiplan>();
                                    var sourceStopPlan = plans.FirstOrDefault(x => x.source == filter.Source.FirstOrDefault());

                                    if (sourceStopPlan != null)
                                    {
                                        foreach (var stopDestinationPlan in plans.Where(x => x.source == filter.Stop.FirstOrDefault()))
                                        {
                                            var multiPlans = _ryanairService.BuildMultiplan(sourceStopPlan, stopDestinationPlan);
                                            res.AddRange(multiPlans);
                                        }
                                    }
                                    lst = res.OfType<IPlan>().ToList();
                                }

                                excelService.CreateSheet(excel, lst, sheet.Item1, sheet.Item2, destinantions, filter.IsCountrySource, height);
                                lst.Clear();
                            }
                        }

                        excel.Save();
                    }

                }

            }).WithNotParsed<CommandLineOptions>(n =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Flight Plan command line parser error : " + string.Join("|", n.Select(x => x.Tag)));
                Console.ForegroundColor = ConsoleColor.White;
            });
        }

        private List<Plan> RunMultiple(FilterProcess filter, Tuple<int, int> sheet, string source)
        {
            var plans = new List<Plan>();
            foreach (var stop in filter.Stop)
            {
                Console.WriteLine($".... Start Processing Source: {source}, Stop Destination: {stop} Month: {sheet.Item1} Year: {sheet.Item2}");

                var plan = Task.Run(() => _ryanairService.GetFlightPlan(source, stop, sheet.Item1, sheet.Item2)).Result;

                if (plan != null && plan.days.Count > 0 && plan.month > 0)
                {
                    plans.Add(plan);
                }

                var singlePlan = RunSingle(filter, sheet, stop, "Stop");

                if (singlePlan != null)
                    plans.AddRange(singlePlan);
            }

            return plans;
        }

        private List<Plan> RunSingle(FilterProcess filter, Tuple<int, int> sheet, string source, string label = null)
        {
            var plans = new List<Plan>();

            foreach (var dest in filter.Destination)
            {
                Console.WriteLine($".... Start Processing {label} Source: {source} Destination: {dest} Month: {sheet.Item1} Year: {sheet.Item2}");

                var plan = Task.Run(() => _ryanairService.GetFlightPlan(source, dest, sheet.Item1, sheet.Item2)).Result;

                if (plan != null && plan.days.Count > 0 && plan.month > 0)
                {
                    plans.Add(plan);
                }
            }

            return plans;
        }

    }
}
