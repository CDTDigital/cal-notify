using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CalNotify.Events.Interfaces;
using CalNotify.Models.User;
using CalNotify.Services;
using Microsoft.EntityFrameworkCore;

namespace CalNotify.Events.Simple
{
    /// <summary>
    ///     Base event class for handling customer related events.
    ///     Utilizes <see cref="Constants.PropetyNames.UserId" /> for name required for json seralization.
    /// </summary>
    [DataContract]
    public class UserRelatedEvent : IUserEvent
    {
        // cache helper
        private GenericUser _user;

        /// <summary>
        ///     MVC Model binding requires an empty constructor in order to properly work.
        /// </summary>
        [Obsolete(Constants.ObsoleteOnlyForBinding, true)]
        public UserRelatedEvent() {}

        /// <summary>
        ///     Initializes our event with the needed customer id.
        /// </summary>
        /// <param name="customerId">The id of the customer this event needs</param>
        public UserRelatedEvent(string customerId)
        {
            CustomerId = customerId;
        }

        /// <summary>
        ///     The customer needed for this event
        /// </summary>
        [DataMember(Name = Constants.PropetyNames.UserId)]
        [Required]
        public string CustomerId { get; set; }

        /// <summary>
        ///     Uses <see cref="CustomerId" /> to try and return the customer. Will return null if not found
        /// </summary>
        /// <remarks>
        ///     NOTE: Does not include any other embedded properties such as history, reviews, or  payment.
        ///     To get all of those properties, see <see cref="GetWithAllData" />
        /// </remarks>
        /// <param name="context">A Database context to use for retieval</param>
        /// <returns>A customer if there is one with the provided id, otherwise null</returns>
        public GenericUser GetUser(BusinessDbContext context)
        {
            if (_user != null)
                return _user;

            return _user = Includes(context.Users).FirstOrDefault(t => t.Id == new Guid(CustomerId));
        }

        /// <summary>
        ///     Base method which allows subclasses to include more embedded properties, i.e. Payment, History, Reviews.
        ///     This implementation doesnt do anything, but subclasses which need more data but are understand
        ///     the performance implications can override this as need be.
        /// </summary>
        /// <param name="query">The eventual list of customers coming from the database</param>
        /// <returns></returns>
        public virtual IQueryable<GenericUser> Includes(IQueryable<GenericUser> query)
        {
            return query.Include(u => u.Address);
        }

    }
}