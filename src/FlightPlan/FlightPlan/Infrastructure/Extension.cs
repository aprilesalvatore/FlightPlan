using System;
using System.Collections.Generic;
using System.Text;

namespace FlightPlan.Infrastructure
{
    public static class Extension
    {
        private static void AppendPipe(this StringBuilder sb)
        {
            if (sb.Length > 0)
                sb.Append(" | ");
        }

        public static string ToExceptionString(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            if (ex != null)
            {
                if (!String.IsNullOrEmpty(ex.Message))
                {
                    sb.AppendFormat("Message = '{0}'", ex.Message.Trim());
                }

                if (!String.IsNullOrEmpty(ex.Source))
                {
                    sb.AppendPipe();

                    sb.AppendFormat("Source = '{0}'", ex.Source.Trim());
                }

                if (!String.IsNullOrEmpty(ex.StackTrace))
                {
                    sb.AppendPipe();

                    sb.AppendFormat("StackTrace = '{0}'", ex.StackTrace.Trim());
                }

                if (ex.InnerException != null)
                {
                    sb.AppendPipe();

                    sb.AppendFormat("Inner Exception = '{0}'", ex.InnerException.ToExceptionString());
                }
            }

            return sb.ToString();
        }
    }
}
