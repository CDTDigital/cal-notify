using System;
using System.Linq;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;
using CalNotifyApi.Services;
using Serilog;

namespace CalNotifyApi.Events
{
    public class CreateOrUpdateCommunicationUserEvent
    {
        

        public GenericUser Process(BusinessDbContext context,
                TempUser tempUser)
        {

            var exisitingUser = context.Users.FirstOrDefault(x => x.PhoneNumber == tempUser.PhoneNumber);
            // we already have an existing user,
            // instead of throwing out an error we will do the right thing and just retoken them
            if (exisitingUser != null)
            {
                return Update(context, tempUser, exisitingUser);
            }
          
            var addr = new Address(tempUser);
            context.Address.Add(addr);

            var genericUser = new GenericUser()
            {
                Name = tempUser.Name,
                Email = tempUser.Email,
                PhoneNumber = tempUser.PhoneNumber,
                Address = addr,
                UserName = string.IsNullOrEmpty(tempUser.Email) ? tempUser.PhoneNumber : tempUser.Email
            };

            return Process(context, genericUser);

        }

        public GenericUser Update(BusinessDbContext context, ITempUser tempUser, GenericUser exisitingUser)
        {

            exisitingUser.Email = tempUser.Email;
            exisitingUser.PhoneNumber = tempUser.PhoneNumber;
            context.SaveChanges();
            Log.Information("Updating  User {exisitingUser}", exisitingUser);
            return exisitingUser;
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
