using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;
using CalNotifyApi.Models.Services;
using HandlebarsDotNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MimeKit;
using Serilog;

namespace CalNotifyApi.Services
{
    public class ValidationSender : ISmsSender, IEmailSender
    {

        private const string TwilioSmsEndpointFormat
            = "https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json";


        private readonly ExternalServicesConfig _config;
        private readonly ILogger<ValidationSender> _log;
        private readonly ITokenMemoryCache _memoryCache;

        private readonly IHostingEnvironment _hostingEnv;

        private static readonly PasswordHasher<string> PasswordHasher = new PasswordHasher<string>();

        #region publicAPI
        public ValidationSender(ExternalServicesConfig config, ILogger<ValidationSender> log, ITokenMemoryCache memoryCache, IHostingEnvironment hostingEnv)
        {
            _config = config;
            _log = log;
            _memoryCache = memoryCache;
            _hostingEnv = hostingEnv;
        }

        public virtual async Task<string> SendValidationToSms(TempUser model)
        {
            var token = SetShortToken(model, TokenType.SmsToken);
            var path = Path.Combine(_hostingEnv.ContentRootPath, "Templates", "sms_validation.hbs");
            var template = File.ReadAllText(path);
            var verificationTemplate = Handlebars.Compile(template);

            var data = new
            {
                url = GetTokenUrl(model),
                name = model.Name ?? "hello"
            };
            await SendMessage(model.PhoneNumber, verificationTemplate(data));

            return token;
        }



        private string SetShortToken(TempUser model, TokenType tokenType)
        {
            var r = new Random((int)DateTime.Now.Ticks);

            // Generate four-digit token
            var token = r.Next(1000, 9999).ToString();
            // Cant forget to set our token
            model.Token = token;
            model.TokenType = tokenType;
            return token;
        }

        private string GetTokenUrl(TempUser model)
        {
            return
                $"{_config.Urls.Backend.Trim('/')}{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/{Constants.ValidationAction}?token={model.Token}";
        }

        private string SetToken(TempUser model, TokenType tokenType)
        {

            var guid = Guid.NewGuid().ToString();
            // Cant forget to set our token
            model.Token = guid;
            model.TokenType = tokenType;
            return guid;
        }



        /// <summary>
        /// Send an sms message using Twilio REST API
        /// </summary>
        /// <param name="toPhoneNumber">E.164 formatted phone number, e.g. +16175551212</param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SendMessage(string toPhoneNumber, string message)
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



        public async Task<string> SendValidationToEmail(TempUser model)
        {
            var token = SetToken(model, TokenType.EmailToken);

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_config.Email.Validation.Name, _config.Email.Validation.Address));
            emailMessage.To.Add(new MailboxAddress(model.Email));
            emailMessage.Subject = "Cal-Notify Validation Link";


            var path = Path.Combine(_hostingEnv.ContentRootPath, "Templates", "email_validation.hbs");
            var template = File.ReadAllText(path);
            var verificationTemplate = Handlebars.Compile(template);
            var data = new
            {
                name = model.Name ?? "hello",
                tokenurl = GetTokenUrl(model),
                helpurl = $"{_config.Urls.Frontend.Trim('/')}/{_config.Pages.HelpPage}"
            };

            var builder = new BodyBuilder();

            builder.HtmlBody = verificationTemplate(data);


            emailMessage.Body = builder.ToMessageBody();
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_config.Email.Validation.Server, _config.Email.Validation.Port, SecureSocketOptions.None).ConfigureAwait(false);
                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(_config.Email.Validation.Username, _config.Email.Validation.Password);

                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }

                return token;

            }
            catch (Exception e)
            {
                Log.Fatal(e, Constants.Messages.EmailValidationFailure);
                throw new ProcessEventException(Constants.Messages.EmailValidationFailure, new List<string>() { e.Message, e.HelpLink, e.Source });
            }

        }

        #endregion
    }
}

