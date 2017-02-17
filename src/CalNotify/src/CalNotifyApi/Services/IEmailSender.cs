using System.Threading.Tasks;
using CalNotify.Models.Interfaces;

namespace CalNotify.Services
{
    public interface IEmailSender
    {
        Task<string> SendValidationToEmail(ITokenAble model);
    }
}