using System.Threading.Tasks;
using CalNotify.Models.Interfaces;

namespace CalNotify.Services
{
    public interface ISmsSender
    {
        Task<string> SendValidationToken(ITokenAble model);
    }
}