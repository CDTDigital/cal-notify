using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CalNotifyApi.Models
{
    [DataContract]
    public class PublishedNotificationLog
    {
        [DataMember(Name = "sent_email")]
        public int SentEmail { get; set; }

        [DataMember(Name ="sent_sms")]
        public int SentSms { get; set; }

        [DataMember(Name = "published")]
        public DateTime Published { get; set; }
    }
}
