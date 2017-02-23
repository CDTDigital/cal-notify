using System.Linq;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Services;

namespace CalNotifyApi.Events
{
    public class EnableUserEvent
    {
        public BaseUser Process(BusinessDbContext context, string validationToken, BaseUser exisitingUser, TempUser tempUser)
        {

         
            if (exisitingUser.Enabled)
            {
                throw new ProcessEventException("User has already been enabled");
            }
           
            if (!string.IsNullOrEmpty(exisitingUser.Email) && tempUser.TokenType == TokenType.EmailToken)
            {
                exisitingUser.ValidatedEmail = true;

            }
            if (!string.IsNullOrEmpty(exisitingUser.PhoneNumber) && tempUser.TokenType == TokenType.SmsToken)
            {
                exisitingUser.ValidatedSms = true;

            }
            exisitingUser.Enabled = true;
            context.SaveChanges();
            return exisitingUser;
        }
    }
}