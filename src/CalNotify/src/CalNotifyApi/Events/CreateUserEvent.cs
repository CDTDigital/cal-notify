using System;
using System.Linq;
using CalNotify.Models.Auth;
using CalNotify.Models.User;
using CalNotify.Services;
using Serilog;

namespace CalNotify.Events
{
    public class CreateUserEvent
    {
        public GenericUser Process(BusinessDbContext context,
                TempUserWithSms tempUser)
        {

            var exisitingUser = context.Users.FirstOrDefault(x => x.PhoneNumber == tempUser.PhoneNumber);
            // we already have an existing user,
            // instead of throwing out an error we will do the right thing and just retoken them
            if (exisitingUser != null)
            {
                return exisitingUser;
            }
            var genericUser = new GenericUser()
            {
                Name = tempUser.Name,
                Email = tempUser.Email,
                PhoneNumber = tempUser.PhoneNumber,
                UserName = string.IsNullOrEmpty(tempUser.Email) ? tempUser.PhoneNumber : tempUser.Email
            };

            return Process(context, genericUser);

        }

        public GenericUser Process(BusinessDbContext context,
            GenericUser user)
        {
            var exisitingUser = context.Users.FirstOrDefault(x => x.PhoneNumber == user.PhoneNumber);
            // we already have an existing user,
            // instead of throwing out an error we will do the right thing and just retoken them
            if (exisitingUser != null)
            {
                return exisitingUser;
            }

            // Set our join date and last login 
            user.JoinDate = user.LastLogin = DateTime.Now;
            context.AllUsers.Add(user);
            context.SaveChanges();
            Log.Information("Created new User {user}", user);
            return user;
        }

    }
}
