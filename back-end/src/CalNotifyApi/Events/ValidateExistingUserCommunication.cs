﻿using System;
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

            var exisitingUser = context.Users.FirstOrDefault(x => x.PhoneNumber == tempUser.PhoneNumber);
            // we already have an existing user,
            // instead of throwing out an error we will do the right thing and just retoken them
            Debug.Assert(exisitingUser != null, "Shouldn't ever be trying to update a null'ed user");

            if (!string.IsNullOrEmpty(exisitingUser.Email) && tempUser.TokenType == TokenType.EmailToken)
            {
                exisitingUser.Email = tempUser.Email;
                exisitingUser.ValidatedEmail = true;

            }
            if (!string.IsNullOrEmpty(exisitingUser.PhoneNumber) && tempUser.TokenType == TokenType.SmsToken)
            {
                exisitingUser.PhoneNumber = tempUser.PhoneNumber;
                exisitingUser.ValidatedSms = true;

            }
            context.SaveChanges();
            Log.Information("Updating  User {exisitingUser}", exisitingUser);
            return exisitingUser;
        }


            

    }
}