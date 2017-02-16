using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CalNotify.Models.Interfaces;
using CalNotify.Utils;
using Microsoft.Extensions.Logging;

namespace CalNotify.Services
{
    public class TwilioSmsSender : ISmsSender
    {

        private const string TwilioSmsEndpointFormat
            = "https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json";

        
        private readonly ExternalServicesConfig _config;
        private readonly ILogger<TwilioSmsSender> _log;

        #region publicAPI
        public TwilioSmsSender(ExternalServicesConfig config, ILogger<TwilioSmsSender> log)
        {
            _config = config;
            _log = log;
        }

        public virtual async Task<string> SendValidationToken(ITokenAble model)
        {
            var token = Extensions.CreateToken();
            model.Token = token;
            var msg = $"Your token is:\n {token}";
            await SendMessage(model.PhoneNumber, msg);
           
            return token;
        }


        /// <summary>
        /// Send an sms message using Twilio REST API
        /// </summary>
        /// <param name="toPhoneNumber">E.164 formatted phone number, e.g. +16175551212</param>
        /// <param name="message"></param>
        /// <returns></returns>
        private  async Task<bool> SendMessage(string toPhoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber))
            {
                throw new ArgumentException("toPhoneNumber was not provided");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("message was not provided");
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = CreateBasicAuthenticationHeader(
                _config.Twillo.Id,
                _config.Twillo.SecretKey);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("To", toPhoneNumber),
                new KeyValuePair<string, string>("From",_config.Twillo.Number),
                new KeyValuePair<string, string>("Body", message)
            };

            var content = new FormUrlEncodedContent(keyValues);

            var postUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    TwilioSmsEndpointFormat,
                    _config.Twillo.Id);

            var response = await client.PostAsync(
                postUrl,
                content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                _log?.LogDebug("success sending sms message to " + toPhoneNumber);
                return true;
            }


            var responseBody = await response.Content.ReadAsStringAsync();
            var logmessage = $"failed to send sms message to {toPhoneNumber} from {_config.Twillo.Number} { response.ReasonPhrase } { responseBody }";
            _log?.LogWarning(logmessage);

            return false;


        }

        #endregion

        #region Internals
        private static AuthenticationHeaderValue CreateBasicAuthenticationHeader(string username, string password)
        {
            return new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                     string.Format("{0}:{1}", username, password)
                     )
                 )
            );
        }

        #endregion
    }
}
