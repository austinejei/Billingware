using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;

namespace Billingware.Common.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class CommonLogger

    {
        /// <summary>
        /// 
        /// </summary>
        public static Logger Logger { get; } = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        public static void Info<T>(string template,
            params object[] parameters)
        {
            Logger.ForContext<T>().Information(template,
                parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        public static void Warn<T>(string template,
            params object[] parameters)
        {
            Logger.ForContext<T>().Warning(template,
                parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        public static void Debug<T>(string template,
            params object[] parameters)
        {
            Logger.ForContext<T>().Debug(template,
                parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception"></param>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        public static void Error<T>(Exception exception,
            string template,
            params object[] parameters)
        {
            Logger.ForContext<T>().Error(exception,
                template,
                parameters);
        }
    }
}
