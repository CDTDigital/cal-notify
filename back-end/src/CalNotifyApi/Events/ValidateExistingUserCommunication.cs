using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CalNotifyApi.Events
{
    public class ValidateExistingUserCommunication
    {
        

        public GenericUser Process(BusinessDbContext context,
                TempUser tempUser)
        {

            var exisitingUser = context.Users.FirstOrDefault(x => x.Id == tempUser.Id);
            // we already have an existing user,
            // instead of throwing out an error we will do the right thing and just retoken them
            Debug.Assert(exisitingUser != null, "Shouldn't ever be trying to update a null'ed user");

            if (!string.IsNullOrEmpty(exisitingUser.Email) && tempUser.TokenType == TokenType.EmailToken)
            {
                // remove old account
                var existingUsername = context.Users.FirstOrDefault(x => x.UserName == tempUser.Email && (x.Id != exisitingUser.Id));
                if (existingUsername != null)
                {
                    context.AllUsers.Remove(existingUsername);
                }
               

                exisitingUser.Email = tempUser.Email;
                exisitingUser.ValidatedEmail = true;

            }
            if (!string.IsNullOrEmpty(exisitingUser.PhoneNumber) && tempUser.TokenType == TokenType.SmsToken)
            {
                // remove old account
                var existingUsername = context.Users.FirstOrDefault(x => x.UserName == tempUser.PhoneNumber && (x.Id != exisitingUser.Id));
                if (existingUsername != null)
                {
                    context.AllUsers.Remove(existingUsername);
                }

                exisitingUser.PhoneNumber = tempUser.PhoneNumber;
                exisitingUser.ValidatedSms = true;

            }
            context.SaveChanges();
            Log.Information("Updating  User {exisitingUser}", exisitingUser);
            return exisitingUser;
        }


            

    }
}
