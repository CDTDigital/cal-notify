using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Services;

namespace CalNotifyApi.Events
{
    [DataContract]
    public class BroadcastNotificationEvent
    {
      /*  [DataMember(Name = "id")]
        public long Id { get; set; }*/


        public void Process(BusinessDbContext context, Notification notification)
        {
            // TODO
        }
    }
}
