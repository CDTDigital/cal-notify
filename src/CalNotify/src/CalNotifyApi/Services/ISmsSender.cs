using System.Threading.Tasks;
using CalNotify.Models.Interfaces;

namespace CalNotify.Services
{
    public interface ISmsSender
    {
        Task<string> SendValidationToSms(ITokenAble model);
    }
}