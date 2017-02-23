using System;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalNotifyApi.Events
{
    public class CheckValidationTokenEvent
    {
        public TempUser  Process(ITokenMemoryCache memoryCache,TokenCheck model)
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

        public TempUser Process(ITokenMemoryCache memoryCache, string model)
        {
            var savedUser = memoryCache.GetForChallenge(model);

            if (savedUser == null)
            {
                throw new CheckValidationTokenException("Customer never received a challenge token");
            }

            if (!memoryCache.IsEqual(savedUser, model))
            {
                throw new CheckValidationTokenException("Customer entered in wrong challenge token");
            }
            return savedUser;
        }
    }

    public class CheckValidationTokenException : ProcessEventException
    {
        public CheckValidationTokenException(string message) : base(message)
        {
        }

        public IActionResult ResponseShellError => ResponseShell.Error(Message);
    }
}
