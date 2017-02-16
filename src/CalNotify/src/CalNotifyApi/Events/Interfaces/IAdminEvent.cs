using CalNotify.Models.Admins;
using CalNotify.Services;

namespace CalNotify.Events.Interfaces
{
    public interface IAdminEvent
    {
        WebAdmin GetAdmin(BusinessDbContext context);

    }
}