using System.Runtime.Serialization;

namespace CalNotify.Models.Responses
{
    [DataContract]
    public class SimpleSuccess
    {
        [DataMember(Name = "success")] public bool Success = true;
    }

    [DataContract]
    public class FailedSucces
    {
        [DataMember(Name = "success")] public bool Success = false;
    }
}