using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Services;
using Microsoft.EntityFrameworkCore;

namespace CalNotifyApi.Events
{
    [DataContract]
    public class BroadcastNotificationEvent
    {
        /*  [DataMember(Name = "id")]
          public long Id { get; set; }*/


        public async Task Process(BusinessDbContext context, ValidationSender sender, Notification notification)
        {

            var queryString = $@"
              SELECT users.""Id"", users.""ValidatedEmail"", users.""ValidatedSms"",  users.""Enabled"", users.""Email"", users.""PhoneNumber"",
                addr.""GeoLocation"", addr.""UserId"",
                noti.""AffectedArea"" FROM public.""Address""  addr, public.""AllUsers"" users, public.""Notifications"" noti
                where ST_Intersects(ST_SetSRID(noti.""AffectedArea"",4326),addr.""GeoLocation"")
                ";

            var foundUsers = new List<Rows>();
            using (var connection = context.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryString;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           foundUsers.Add(new Rows()
                           {
                              Id = (Guid)reader[0],
                              ValidatedEmail = (bool) reader[1],
                              ValidatedSms = (bool)reader[2],
                              Enabled = (bool) reader[3],
                              Email = (string)reader[4],
                              PhoneNumber = (string)reader[5]
                           });
                        }

                    }
                }


                // Use the notification bounds and find the users which fall into those bounds
#pragma warning disable 4014
                 Broadcast(sender, foundUsers, notification);
#pragma warning restore 4014

                // Use their communication information which has been validated to fire off the messages

                // Collect stats with the broadcasting
            }
        }


        internal async Task Broadcast(ValidationSender sender, List<Rows> users, Notification notification)
        {
            foreach (var user in users)
            {
                if (user.ValidatedSms && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    await sender.SendMSMessage(user.PhoneNumber, notification);
                }
                if (user.ValidatedEmail && !string.IsNullOrEmpty(user.Email))
                {
                    await sender.SendEmailMessage(user.Email, notification);
                }
            }
            return;
        }

        internal class Rows
        {
            public System.Guid Id { get; set; }

            public string Email { get; set; }
          
            public bool Enabled { get; set; }

            public bool EnabledEmail { get; set; }

            public bool ValidatedEmail { get; set; }

            public bool ValidatedSms { get; set; }

            public bool EnabledSms { get; set; }

            public string PhoneNumber { get; set; }
        }
    }

   
}
