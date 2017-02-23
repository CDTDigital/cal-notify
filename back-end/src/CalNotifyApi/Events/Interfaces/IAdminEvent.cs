using CalNotifyApi.Models.Admins;
using CalNotifyApi.Services;

namespace CalNotifyApi.Events.Interfaces
{
    public interface IAdminEvent
    {
        WebAdmin GetAdmin(BusinessDbContext context);

    }
}