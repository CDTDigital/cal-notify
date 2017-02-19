using System.Threading.Tasks;
using CalNotifyApi.Models.Interfaces;

namespace CalNotifyApi.Services
{
    public interface IEmailSender
    {
        Task<string> SendValidationToEmail(ITempUser model);
    }
}