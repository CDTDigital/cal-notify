using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CalNotifyApi.Controllers
{
    
    [Route(Constants.V1Prefix + "/" + Constants.UtilsEndpoint)]
    public class UtilResources: Controller
    {

 

        [HttpGet("status"), HttpGet("/")]
        [ProducesResponseType(typeof(string), 200)]
        public virtual IActionResult Status()
        {
            return new JsonResult(new StatusInfo() { Version = Constants. V1Prefix});
        }

        [DataContract]
        public class StatusInfo
        {
            [DataMember(Name = "version")]
            public string Version { get; set; }
        }

    }
}
