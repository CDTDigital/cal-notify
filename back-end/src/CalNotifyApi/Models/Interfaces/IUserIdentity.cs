using System;

namespace CalNotifyApi.Models.Interfaces
{
    public interface IUserIdentity
    {
        Guid Id { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
    }
}