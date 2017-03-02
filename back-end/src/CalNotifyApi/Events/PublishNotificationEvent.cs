using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CalNotifyApi.Events
{
    [DataContract]
    public class PublishNotificationEvent
    {
        /*  [DataMember(Name = "id")]
          public long Id { get; set; }*/


        public Notification Process(BusinessDbContext context, string adminId, ValidationSender sender, string connectionString, Notification notification)
        {

            var queryString = $@"
              SELECT DISTINCT ON(users.""Id"") users.""Id"", users.""ValidatedEmail"", users.""ValidatedSms"",  users.""Enabled"", users.""Email"", users.""PhoneNumber"",  users.""EnabledEmail"", users.""EnabledSms"",
                addr.""GeoLocation"", addr.""UserId"",
                noti.""AffectedArea"" FROM public.""Address""  addr, public.""AllUsers"" users, public.""Notifications"" noti
                where ST_Intersects(ST_SetSRID(noti.""AffectedArea"",4326),addr.""GeoLocation"") and users.""Id"" = addr.""UserId"" and  noti.""Id"" = {notification.Id}
                ";

            var foundUsers = new List<Rows>();
            try
            {
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
                                    ValidatedEmail = (bool)reader[1],
                                    ValidatedSms = (bool)reader[2],
                                    Enabled = (bool)reader[3],
                                    Email = (reader[4] ?? "").ToString(),
                                    PhoneNumber = (reader[5] ?? "").ToString(),
                                    EnabledEmail = (bool)reader[6],
                                    EnabledSms = (bool)reader[7]

                                });
                            }

                        }
                    }


                    context.Notifications.Update(notification);
                    notification.Status = NotiStatus.Published;
                    notification.Published = DateTime.Now;
                    notification.PublishedById = new Guid(adminId);
                    context.SaveChanges();

                    Log.Information("Sending out notification to {count} users", foundUsers.Count);

                    // Use the notification bounds and find the users which fall into those bounds
#pragma warning disable 4014
                    Broadcast(connectionString, sender, foundUsers, notification);
#pragma warning restore 4014

                    // Use their communication information which has been validated to fire off the messages

                    // Collect stats with the broadcasting

                    return notification;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        private async Task Broadcast(string connectionString, ValidationSender sender, List<Rows> users, Notification notification)
        {
            var options = new DbContextOptionsBuilder<BusinessDbContext>();
            options.UseNpgsql(connectionString);

         
            using (var context = new BusinessDbContext(options.Options))
            {
                foreach (var user in users)
                {
                    try
                    {
                        if (user.ValidatedSms && user.EnabledSms && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                        {
                            Log.Information("Sending out notification to {phone}", user.PhoneNumber);
                            await sender.SendMSMessage(user.PhoneNumber, notification);
                            context.NotificationLog.Add(new BroadCastLogEntry()
                            {
                                NotificationId = notification.Id.Value,
                                UserId = user.Id,
                                Type = LogType.Email
                            });
                        }
                        if (user.ValidatedEmail && user.EnabledEmail && !string.IsNullOrEmpty(user.Email))
                        {
                            Log.Information("Sending out notification to {email}", user.Email);
                            await sender.SendEmailMessage(user.Email, notification);
                            context.NotificationLog.Add(new BroadCastLogEntry()
                            {
                                NotificationId = notification.Id.Value,
                                UserId = user.Id,
                                Type = LogType.Sms
                            });
                        }
                       
                        context.SaveChanges();

                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error when broadcasting to user {user}", user);
                    }

                }
                return;
            }
               
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
