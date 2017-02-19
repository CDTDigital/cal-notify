using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace CalNotifyApi.Models.Responses
{
    /// <summary>
    ///     Our common wrapper around all of our API's responses
    /// </summary>
    [DataContract]
    public class ResponseShell<T>
    {
        /// <summary>
        ///     The resulting data from this response, if any.
        /// </summary>
        [DataMember(Name = "result")]
        [JsonProperty(Order = 2)]
        public T Result { get; set; }

        /// <summary>
        ///     Holds the metainformation related to this response
        /// </summary>
        [DataMember(Name = "meta")]
        [JsonProperty(Order = 1)]
        public Meta Meta { get; set; }
    }

    /// <summary>
    ///     Our static helpers to output our shell and its required data
    /// </summary>
    public class ResponseShell
    {
        public static IActionResult NotFound(string message)
        {
            return NotFound(message, new List<string>());
        }

        public static IActionResult NotFound(string message, List<string> errorList)
        {
            return Error((int) HttpStatusCode.NotFound, "The resource was not found", errorList);
        }

        public static IActionResult NotImplementated()
        {
            return Error(501, "Not implemented yet.", new List<string>
            {
                "Low Priority"
            });
        }

        public static string AuthErrorString(Meta meta)
        {
            var result = new ResponseShell<string>
            {
                Meta = meta
            };
            return JsonConvert.SerializeObject(result, Constants.CreateJsonSerializerSettings());
        }

        public static IActionResult AuthError(Meta meta)
        {
            var result = AuthErrorString(meta);
            return new ObjectResult(result)
            {
                StatusCode = meta.Code
            };
        }


        public static IActionResult Error(string message)
        {
            return Error(400, message, new List<string>());
        }

        public static IActionResult Error(string message, IdentityResult identityResult)
        {
            var errors = identityResult.Errors.Select(er => er.Description).ToList();
            return Error(400, message, errors);
        }


        public static IActionResult Error(int statusCode, string message)
        {
            return Error(statusCode, message, new List<string>());
        }

        public static IActionResult Error(string msg, List<string> errors)
        {
            return Error(400, msg, errors);
        }


        public static IActionResult Error(int statusCode, string message, ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(m => m.Errors.Select(er => er.ErrorMessage)).ToList();
            return Error(statusCode, message, errors);
        }

        public static IActionResult Ok<T>(T data)
        {
            var result = new ResponseShell<T>
            {
                Meta = new Meta {Code = 200},
                Result = data
            };

            return new ObjectResult(result)
            {
                StatusCode = 200
            };
        }

 


        public static IActionResult Ok()
        {
            return Ok(new SimpleSuccess());
        }

        public static IActionResult ErrorUnexpected(Exception e)
        {
            return Error(Constants.StatusCodes.ErrorUnexpcetedStatusCode, Constants.Messages.UnexpectedErrorMsg, new List<string>
            {
                  "High Priority",
                e.Message,
                e.StackTrace,
                e?.InnerException.Message,
                e?.InnerException.StackTrace
                
            });
        }

        #region Helpers

        private static IActionResult Error(int statusCode, string message, List<string> details)
        {
            var result = new ResponseShell<string>
            {
                Meta = new Meta
                {
                    Message = message,
                    Details = details,
                    Code = statusCode
                }
            };

            return new ObjectResult(result)
            {
                StatusCode = statusCode
            };
        }

        #endregion
    }
}