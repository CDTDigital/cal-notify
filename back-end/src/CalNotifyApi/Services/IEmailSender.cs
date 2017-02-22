using System.Threading.Tasks;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;

namespace CalNotifyApi.Services
{
    public interface IEmailSender
    {
        Task<string> SendValidationToEmail(TempUser model);
    }
}