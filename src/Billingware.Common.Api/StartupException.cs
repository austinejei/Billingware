using System;

namespace Billingware.Common.Api
{
    /// <summary>
    /// </summary>
    public class StartupException : Exception
    {
        /// <summary>
        /// </summary>
        public StartupException(string mesg) : base(mesg)
        { }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public StartupException(string message,
            Exception innerException) : base(message,
            innerException)
        { }
    }
}