using System.Threading.Tasks;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;

namespace CalNotifyApi.Services
{
    public interface ISmsSender
    {

        Task<string> SendValidationToSms(TempUser model);
    }
}