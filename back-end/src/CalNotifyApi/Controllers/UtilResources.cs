using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CalNotifyApi.Controllers
{
    
    [Route(Constants.V1Prefix + "/" + Constants.UtilsEndpoint)]
    public class UtilResources: Controller
    {

 
        /// <summary>
        /// Exposes a simple version and information endpoint. For health checks 
        /// </summary>
        /// <returns></returns>
        [HttpGet("status"), HttpGet("/")] // set as api root for easy monitoring and discovery
        [ProducesResponseType(typeof(string), 200)]
        public virtual IActionResult Status()
        {
            return new JsonResult(new StatusInfo()
            {
                Status ="Live",
                Version = Constants. V1Prefix,
                Information = "Cal-Notify API. For more information visit the relative url: swagger/index.html"
            });
        }


        [DataContract]
        public class StatusInfo
        {
            [DataMember(Name = "version")]
            public string Version { get; set; }
            [DataMember(Name = "information")]
            public string Information { get; set; }

            [DataMember(Name = "status")]
            public string Status { get; set; }
        }

    }
}
