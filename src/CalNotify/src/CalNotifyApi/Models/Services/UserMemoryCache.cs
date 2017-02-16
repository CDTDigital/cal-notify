using System;
using CalNotify.Models.Auth;
using CalNotify.Models.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CalNotify.Models.Services
{
    public interface ITokenMemoryCache
    {
        void SetForChallenge(TempUserWithSms model);
        TempUserWithSms GetForChallenge(ITokenAble model);
        bool IsEqual(ITokenAble a, ITokenAble b);
    }


    public class TokenMemoryCache : ITokenMemoryCache
    {
        private readonly IMemoryCache _cache;

        public TokenMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public virtual void SetForChallenge(TempUserWithSms model)
        {
            // Hold our token and model for a while to give our user a chance to validate their info
            if (string.IsNullOrEmpty(model.Token))
                throw new Exception("Should always recieve a token");
            _cache.Set(model.PhoneNumber, model, TimeSpan.FromMinutes(5));
        }

        public virtual TempUserWithSms GetForChallenge(ITokenAble model)
        {
            return _cache.Get<TempUserWithSms>(model.PhoneNumber);
        }

        public  bool IsEqual(ITokenAble a, ITokenAble b)
        {
            return a.Token == b.Token && a.PhoneNumber == b.PhoneNumber;
        }
    }
}