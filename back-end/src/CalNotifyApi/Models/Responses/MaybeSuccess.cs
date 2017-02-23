using System.Runtime.Serialization;

namespace CalNotifyApi.Models.Responses
{
    [DataContract]
    public class MaybeSuccess
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }



    }
}