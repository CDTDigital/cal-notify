using CalNotifyApi.Models;
using CalNotifyApi.Services;

namespace CalNotifyApi.Events.Interfaces
{
    public interface IUserEvent
    {
        GenericUser GetUser(BusinessDbContext context);
    }
}