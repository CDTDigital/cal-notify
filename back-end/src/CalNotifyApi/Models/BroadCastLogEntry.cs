using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalNotifyApi.Models
{
    public class BroadCastLogEntry
    {
        public Guid UserId { get; set; }

        public long NotificationId { get; set; }
    }
}
