using System;
using CalNotify.Models.Auth;
using CalNotify.Models.Responses;
using CalNotify.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalNotify.Events
{
    public class CheckValidationTokenEvent
    {
        public TempUserWithSms  Process(ITokenMemoryCache memoryCache, TokenCheck model)
        {
            var savedUser = memoryCache.GetForChallenge(model);

            if (savedUser == null)
            {
                throw new CheckValidationTokenException("Customer never recieved a challenge token");
            }

            if (!memoryCache.IsEqual(savedUser, model))
            {
                throw new CheckValidationTokenException("Customer entered in wrong challenge token");
            }

            return savedUser;
        }
    }

    public class CheckValidationTokenException : Exception
    {
        public CheckValidationTokenException(string message) : base(message)
        {
        }

        public IActionResult ResponseShellError => ResponseShell.Error(Message);
    }
}
