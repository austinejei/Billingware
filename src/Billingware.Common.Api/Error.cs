using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Billingware.Common.Api
{
    /// <summary>
    /// API Error response
    /// </summary>
    [DataContract(Name = "error")]
    public class Error
    {
        /// <summary>
        /// the parameter that throws an error
        /// </summary>
        [DataMember(Name = "param")]
        public string Param { get; set; }

        /// <summary>
        /// the value set for the parameter during binding
        /// </summary>
        [DataMember(Name = "value")]
        public string Value { get; set; }
        /// <summary>
        /// the errors messages
        /// </summary>
        [DataMember(Name = "messages")]
        public List<string> Messages { get; set; }
    }
}