using System;

namespace FlightPlan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new RunnerContext();
            context.Run(args);
        }
    }
}
