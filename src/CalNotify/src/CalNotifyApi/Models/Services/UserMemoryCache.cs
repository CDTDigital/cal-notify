using System;
using CalNotify.Models.Auth;
using CalNotify.Models.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CalNotify.Models.Services
{
    public interface ITokenMemoryCache
    {
        void SetForChallenge(ITokenAble model);

        ITokenAble GetForChallenge(ITokenAble model);
        ITokenAble GetForChallenge(string model);
        bool IsEqual(ITokenAble a, ITokenAble b);
        bool IsEqual(ITokenAble a, string b);

    }


    public class TokenMemoryCache : ITokenMemoryCache
    {
        private readonly IMemoryCache _cache;

        public TokenMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public virtual void SetForChallenge(ITokenAble model)
        {
            // Hold our token and model for a while to give our user a chance to validate their info
            if (string.IsNullOrEmpty(model.Token))
                throw new Exception("Should always recieve a token");
            _cache.Set(model.Token, model, TimeSpan.FromMinutes(5));
        }

        public virtual ITokenAble GetForChallenge(ITokenAble model)
        {
            return _cache.Get<ITokenAble>(model.Token);
        }

        public virtual ITokenAble GetForChallenge(string  token)
        {
            return _cache.Get<ITokenAble>(token);
        }

        public  bool IsEqual(ITokenAble a, ITokenAble b)
        {
            return a.Token == b.Token && (a.PhoneNumber == b.PhoneNumber) && (a.Email == b.Email);
        }

        public bool IsEqual(ITokenAble a, string token)
        {
            return a.Token == token;
        }
    }
}