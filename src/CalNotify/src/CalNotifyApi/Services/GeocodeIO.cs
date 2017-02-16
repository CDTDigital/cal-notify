using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalNotify.Events.Exceptions;
using CalNotify.Models.Addresses;
using CalNotify.Models.Responses;
using CalNotify.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CalNotify.Services
{
    public class GeocodeIO
    {
        private const string GeoCodeEndpoint = "https://api.geocod.io/v1/geocode";


        private readonly BusinessDbContext _context;
        private readonly ILogger<GeocodeIO> _logger;
        private readonly string ApiKey;

        public GeocodeIO(ExternalServicesConfig config, BusinessDbContext context, ILogger<GeocodeIO> logger)
        {
            _context = context;
            _logger = logger;
            ApiKey = config.Geocodio.SecretKey;
        }


        public async Task<GeocodeioResult> SingleAddress(string q)
        {
            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("api_key", ApiKey),
                new KeyValuePair<string, string>("q", q),
            });

            var query = queryParams.ReadAsStringAsync().Result;

            using (var client = new HttpClient())
            {
                var request = client.GetAsync($"{GeoCodeEndpoint}?{query}");
                // TODO Collect how many calls we have made today, and send alerts when we are approaching our limits
                var response = await request;
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("Failed to complete a Geocod.io lookup for {Message}", q);
                    throw new GeocodeIOProcessException(response.ReasonPhrase);
                }

                var geocodeResult = JsonConvert.DeserializeObject<GeocodeioQueryWrapper>(response.Content.ToString());

                return geocodeResult.Results.FirstOrDefault();
            }
        }


        public async Task<GeocodeioResult> SingleAddress(IAddress addr)
        {
            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("api_key", ApiKey),
                new KeyValuePair<string, string>("street", addr.Street),
                new KeyValuePair<string, string>("city", addr.City),
                new KeyValuePair<string, string>("state", addr.State),
                new KeyValuePair<string, string>("postal_code", addr.Zip),

            });

            var query = queryParams.ReadAsStringAsync().Result;

            using (var client = new HttpClient())
            {
                var request = client.GetAsync($"{GeoCodeEndpoint}?{query}");
                // TODO Collect how many calls we have made today, and send alerts when we are approaching our limits
                var response = await request;
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("Failed to complete a Geocod.io lookup for {@Address}", addr);
                    throw new GeocodeIOProcessException(response.ReasonPhrase);
                }

                var geocodeResult = JsonConvert.DeserializeObject<GeocodeioQueryWrapper>(response.Content.ToString());

                return geocodeResult.Results.FirstOrDefault();
            }


        }
    }

    public class GeocodeIOProcessException : Exception, IProcessEventException
    {
        public GeocodeIOProcessException(string message) : base(message)
        {
        }

        public IActionResult ResponseShellError => ResponseShell.Error(Message);
    }


}
