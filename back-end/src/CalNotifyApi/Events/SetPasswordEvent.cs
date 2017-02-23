using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CalNotifyApi.Events
{
    [DataContract]
    public class SetPasswordEvent
    {
        [DataMember(Name ="token")]
        public string Token { get; set; }

        [DataMember(Name ="password"), Required, MinLength(6)]
        public string Password { get; set; }
    }
}
