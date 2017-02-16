using System.Runtime.Serialization;

namespace CalNotify.Models.Responses
{
    [DataContract]
    public class MaybeSuccess
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }



    }
}