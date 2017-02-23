using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CalNotifyApi.Events.Interfaces;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Services;

namespace CalNotifyApi.Events.Simple
{
    [DataContract]
    public class AdminRelatedEvent : IAdminEvent
    {
        // cache helper
        private WebAdmin _admin;

        /// <summary>
        ///     MVC Model binding requires an empty constructor in order to properly work.
        /// </summary>
        [Obsolete(Constants.ObsoleteOnlyForBinding, true)]
        public AdminRelatedEvent() { }

        /// <summary>
        ///     Initializes our event with the needed admin id.
        /// </summary>
        /// <param name="adminId">The id of the admin this event needs</param>
        public AdminRelatedEvent(string adminId)
        {
            AdminId = adminId;
        }


        [DataMember(Name = Constants.PropetyNames.AdminId)]
        [Required]
        public string AdminId { get; set; }


        public WebAdmin GetAdmin(BusinessDbContext context)
        {
            if (_admin != null)
            {
                return _admin;
            }
            return _admin = context.Admins.FirstOrDefault(x => x.Id == new Guid(AdminId));
        }
    }
}
