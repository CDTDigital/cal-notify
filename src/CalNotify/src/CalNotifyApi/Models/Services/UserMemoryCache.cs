using System;
using CalNotifyApi.Models.Auth;
using Microsoft.Extensions.Caching.Memory;

namespace CalNotifyApi.Models.Services
{
    public interface ITokenMemoryCache
    {
        void SetForChallenge(Auth.TempUser model);
        TempUser GetForChallenge(string token);
        TempUser GetForChallenge(TempUser model);
        bool IsEqual(Auth.TempUser a, Auth.TempUser b);
        bool IsEqual(Auth.TempUser a, string b);

        bool IsEqual(Auth.TempUser a, TokenCheck token);


        TempUser GetForChallenge(TokenCheck model);
    }


    public class TokenMemoryCache : ITokenMemoryCache
    {
        private readonly IMemoryCache _cache;

        public TokenMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public virtual void SetForChallenge(TempUser model)
        {
            // Hold our token and model for a while to give our user a chance to validate their info
            if (string.IsNullOrEmpty(model.Token))
                throw new Exception("Should always recieve a token");
            _cache.Set(model.Token, model, TimeSpan.FromMinutes(5));
        }

        public virtual TempUser GetForChallenge(TempUser model)
        {
            return _cache.Get<TempUser>(model.Token);
        }

    

        public virtual TempUser GetForChallenge(string  token)
        {
            return _cache.Get<TempUser>(token);
        }

        public  bool IsEqual(TempUser a, TempUser b)
        {
            return a.Token == b.Token && (a.PhoneNumber == b.PhoneNumber) && (a.Email == b.Email);
        }

        public bool IsEqual(TempUser a, string token)
        {
            return a.Token == token;
        }

        public bool IsEqual(TempUser a, TokenCheck token)
        {
            return a.Token == token.Token;
        }

        public TempUser GetForChallenge(TokenCheck model)
        {
            return _cache.Get<TempUser>(model.Token);
        }
    }
}