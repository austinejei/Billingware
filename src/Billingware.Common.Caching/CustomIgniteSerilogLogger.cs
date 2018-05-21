using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core.Log;
using Serilog;
using Serilog.Core;
using ILogger = Apache.Ignite.Core.Log.ILogger;

namespace Billingware.Common.Caching
{
    public class CustomIgniteSerilogLogger : ILogger
    {
        protected static readonly Logger Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();
        public void Log(LogLevel level,
            string message,
            object[] args,
            IFormatProvider formatProvider,
            string category,
            string nativeErrorInfo,
            Exception ex)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    Logger.Debug(message, args);
                    break;
                case LogLevel.Debug:
                    Logger.Debug(message, args);
                    break;
                case LogLevel.Info:
                    Logger.Information(message, args);
                    break;
                case LogLevel.Warn:
                    Logger.Warning(message, args);
                    break;
                case LogLevel.Error:
                    Logger.Error(message, args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level),
                        level,
                        null);
            }
        }

        public bool IsEnabled(LogLevel level) { return true; }
    }
}
