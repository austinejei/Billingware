using System.Runtime.Serialization;

namespace Billingware.Common.Api
{
    /// <summary>
    ///  API response object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "response")]
    public class ApiResponse<T>
    {
        ///// <summary>
        /////  The status of the response
        /////  success : stands for successful request
        /////  failed: stands for failed request
        /////  error: stands for error
        ///// </summary>
        //[DataMember(Name = "status")]
        //public string Status { get; set; }


        /// <summary>
        ///  The generic message from the service
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// The response data
        /// </summary>
        [DataMember(Name = "data")]
        public T Data { get; set; }
    }
}