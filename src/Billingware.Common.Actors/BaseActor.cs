using System;
using Akka.Actor;
using Akka.Event;

namespace Billingware.Common.Actors
{
    public class BaseActor : ReceiveActor
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly ILoggingAdapter Logger = Context.GetLogger();


        protected void Publish(object @event) { Context.Dispatcher.EventStream.Publish(@event); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="format"></param>
        /// <param name="cause"></param>
        /// <param name="args"></param>
        protected void Log(CommonLogLevel logLevel, string format, Exception cause, params object[] args)
        {
            switch (logLevel)
            {
                case CommonLogLevel.Debug:
                    Logger.Debug(format, args);
                    break;
                case CommonLogLevel.Error:
                    Logger.Error(cause, format, args);
                    break;
                case CommonLogLevel.Info:
                    Logger.Info(format, args);
                    break;
                case CommonLogLevel.Warn:
                    Logger.Warning(format, args);
                    break;
            }

        }
    }
}