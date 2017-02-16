using CalNotify.Models.User;
using CalNotify.Services;

namespace CalNotify.Events.Interfaces
{
    public interface IUserEvent
    {
        GenericUser GetUser(BusinessDbContext context);
    }
}