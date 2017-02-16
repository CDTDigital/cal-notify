using System.Collections.Generic;
using System.Runtime.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotify.Models.Responses
{
    /// <summary>
    /// For each and every request the response will include a <see cref="Meta" /> 
    /// </summary>
    [DataContract]
    [SwaggerSchemaFilter(typeof(ExampleMetaObject))]
    public class Meta
    {

        /// <summary>
        ///   The status code for the response. 
        /// Any status code besides 200 should be thought of as an error. Codes in the 4xx range are related 
        /// to issues caused by clients, while codes in the 5xx range are server related an no fault of clients.
        ///  </summary>
        [DataMember(Name = "code")]
        public int Code { get; set; }

        /// <summary>
        ///    The primary message which is set for any response outside of the 2xx range.
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        ///     If there is multiple errors which can't be described in the "message" then they will be listed here
        /// </summary>
        [DataMember(Name = "details")]
        public List<string> Details { get; set; }
    }
}