using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CalNotifyApi.Events
{
    public class CreateDisabledUserAccount
    {

        public async Task<GenericUser> Process(IHostingEnvironment hostingEnvironment, ITokenMemoryCache memoryCache, BusinessDbContext context, ValidationSender validation, TempUser tempUser)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                // Check out our users, if we already someone, then no need to validate, its just an error
                var check = await context.Users.AnyAsync(u => u.PhoneNumber == tempUser.PhoneNumber || u.Email == tempUser.Email);
                if (check)
                {
                    throw new ProcessEventException("GenericUser already has an account");
                }

                // Also need to check if their is any pending validation and in that case we would not take this info
                var stillNeedValidation =
                    await context.AllUsers.AnyAsync(
                        u =>
                            (u.PhoneNumber == tempUser.PhoneNumber || u.Email == tempUser.Email) &&
                            u.Enabled == false);

                if (stillNeedValidation)
                {
                    throw new ProcessEventException("User needs to confirm exisiting validation");
                }

                // Create a temporary account now, just keep account disabled untill verified.
                // TODO: background cron job which purges accounts unvalidated after xxx amount of time

                var addr = new Address(tempUser);
                context.Address.Add(addr);

                var user = new GenericUser()
                {
                    Name = tempUser.Name,
                    Email = tempUser.Email,
                    PhoneNumber = tempUser.PhoneNumber,
                    Address = addr,
                    UserName = string.IsNullOrEmpty(tempUser.Email) ? tempUser.PhoneNumber : tempUser.Email
                };

                // Set our join date and last login 
                user.JoinDate = user.LastLogin = DateTime.Now;
                context.AllUsers.Add(user);
                context.SaveChanges();
                Log.Information("Created a new user, unvalidated {user}", user);

                // Need to set our tempUser id to our user
                tempUser.Id = user.Id;

                // TODO: Remove any chance of this obvious inserurity prior to real production usage via removing the check altogther.
                if (Constants.Testing.CheckIfOverride(tempUser) && (hostingEnvironment.IsDevelopment() || hostingEnvironment.IsEnvironment("Testing")))
                {
                    tempUser.Token = Constants.Testing.TestValidationToken;
                    // Hold our token and tempUser for a while to give our user a chance to validate their info
                    memoryCache.SetForChallenge(tempUser);
                    transaction.Commit();
                    return user;
                }

                // We prefer to send validation through email but will send through sms if needed
                if (!string.IsNullOrWhiteSpace(tempUser.Email))
                {
                    Log.Information("Sending Email Validation to {user}", user);

                    await validation.SendValidationToEmail(tempUser);
                    memoryCache.SetForChallenge(tempUser);

                }
                else if (!string.IsNullOrWhiteSpace(tempUser.PhoneNumber))
                {
                    Log.Information("Sending SMS Validation to {user}", user);

                    await validation.SendValidationToSms(tempUser);
                    memoryCache.SetForChallenge(tempUser);


                }

                transaction.Commit();
                return user;
            }

        }
    }
}
