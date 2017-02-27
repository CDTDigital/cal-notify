using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CalNotifyApi.Models
{
    [DataContract]
    public class BroadCastLogEntry
    {
        [Key]
        [DataMember(Name ="id")]
        public long? Id { get; set; }

        [DataMember(Name = "user_id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "notification_id")]
        public long NotificationId { get; set; }

        [DataMember(Name = "type")]
        public LogType Type { get; set; }
    }


    public enum LogType
    {
        Email,
        Sms
    }
}
