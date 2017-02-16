using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CalNotify.Controllers
{
    
    [Route(Constants.V1Prefix + "/" + Constants.UtilsEndpoint)]
    public class UtilResources: Controller
    {

 

        [HttpGet("status")]
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
