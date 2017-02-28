using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using Newtonsoft.Json;
using NpgsqlTypes;
using Serilog;

namespace CalNotifyApi.Events
{
    public class PullFromSourcesEvent
    {

        private const string USGS =
            "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/EarthquakesFromLastSevenDays/MapServer/0/query?text=&geometry=&geometryType=esriGeometryPoint&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&objectIds=&where=magnitude+%3E+5.5&time=&returnCountOnly=false&returnIdsOnly=false&returnGeometry=true&maxAllowableOffset=&outSR=&outFields=*&f=pjson";

        private const string NOAA =
            "https://idpgis.ncep.noaa.gov/arcgis/rest/services/NWS_Observations/ahps_riv_gauges/MapServer/0/query?where=objectid+%3E+0+AND+state+%3D+%27CA%27&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=pjson";

        public async Task Process(BusinessDbContext context)
        {
            try
            {

                await GetNOAA(context);
                await GetUSGS(context);

            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        private async Task GetNOAA(BusinessDbContext context)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(NOAA);


                if (!response.IsSuccessStatusCode)
                {
                    Log.Error("Failed to pull from RIVERSOURCE");
                }

                var admin = context.Admins.FirstOrDefault();
                if (admin == null)
                {
                    throw new ProcessEventException("Could not find an admin to create new events");
                }
                var resultStr = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RiversourceWrapper>(resultStr);

                foreach (var feature in result.Features)
                {
                    var attr = feature.Attributes;

                    double observed;
                    double action;
                    if (!Double.TryParse(attr.Observed, out observed) || !Double.TryParse(attr.Action, out action))
                    {
                        continue;
                    }
                    if (observed < action)
                    {
                        continue;
                    }
                    var existing = context.Notifications.FirstOrDefault(x => x.SourceId == "NOAA" + attr.Id);
                    if (existing != null)
                    {
                        continue;
                    }

                    DateTime created;
                    var didGetDate = DateTime.TryParse(attr.ObserveDateTime, out created);

                    var noti = new Notification()
                    {
                        Title = attr.Waterbody,
                        Details = attr.Url,
                        Location = new PostgisPoint(attr.Longititude, attr.Latitude) { SRID = Constants.SRID },
                        Category = Category.Flood,
                        Source = "NOAA",
                        SourceId = "NOAA" + attr.Id,
                        Severity = Severity.Emergency,
                        Status = NotiStatus.New,
                        Created = (didGetDate) ? created : DateTime.Now,
                        AuthorId = admin.Id

                    };

                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }

            }
        }

        private async Task GetUSGS(BusinessDbContext context)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(USGS);


                if (!response.IsSuccessStatusCode)
                {
                    Log.Error("Failed to pull from USGS");
                }

                var admin = context.Admins.FirstOrDefault();
                if (admin == null)
                {
                    throw new ProcessEventException("Could not find an admin to create new events");
                }

                var resultStr = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EarthquakeSourceWrapper>(resultStr);
                foreach (var feature in result.Features)
                {
                    var attr = feature.Attributes;

                    var existing = context.Notifications.FirstOrDefault(x => x.SourceId == "USGS" + attr.Id);
                    if (existing != null)
                    {
                        continue;
                    }

                    DateTime created;
                    var didGetDate = DateTime.TryParse(attr.ObserveDateTime, out created);
                    var noti = new Notification()
                    {
                        Title = attr.Magnitude + "Earthquake",
                        Details = attr.Region,
                        Location = new PostgisPoint(attr.Longititude, attr.Latitude) { SRID = Constants.SRID },
                        Category = Category.Earthquake,
                        Source = "USGS",
                        SourceId = "USGS" + attr.Id,
                        Severity = Severity.Emergency,
                        Status = NotiStatus.New,
                        Created = (didGetDate) ? created : DateTime.Now,
                        AuthorId = admin.Id

                    };
                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }
            }
        }
    }


   
    [DataContract]
    public class EarthquakeSourceWrapper
    {

        [DataMember(Name = "features")]
        public List<EarthQuakeFeatureWrapper> Features { get; set; }


    }

    [DataContract]
    public class EarthQuakeFeatureWrapper
    {

        [DataMember(Name = "attributes")]
        public EarthQuakeAttribute Attributes { get; set; }
    }

    [DataContract]
    public class EarthQuakeAttribute
    {
        [DataMember(Name = "objectid")]
        public string Id { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longititude { get; set; }

        [DataMember(Name = "region")]
        public string Region { get; set; }

        [DataMember(Name ="magnitude")]
        public double Magnitude { get; set; }


        [DataMember(Name = "datetime")]
        public string ObserveDateTime { get; set; }
    }


    [DataContract]
    public class RiversourceWrapper
    {
        
        [DataMember(Name = "features")]
        public List<RiversourceFeatureWrapper> Features { get; set; }


    }

  


    [DataContract]
    public class RiversourceFeatureWrapper
    {
        [DataMember(Name = "attributes")]
        public RiversourceAttribute Attributes { get; set; }
    }

    [DataContract]
    public class RiversourceAttribute
    {
        [DataMember(Name = "objectid")]
        public string Id { get; set; }
        [DataMember(Name = "waterbody")]
        public string Waterbody { get; set; }

        [DataMember(Name = "observed")]
        public string Observed { get; set; }


        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longititude { get; set; }

        [DataMember(Name = "obstime")]
        public string  ObserveDateTime { get; set; }

        [DataMember(Name = "units")]
        public string Units { get; set; }

        [DataMember(Name ="flood")]
        public string Flood { get; set; }

        [DataMember(Name ="action")]
        public string Action { get; set; }


        [DataMember(Name = "url")]
        public string Url { get; set; }

    }
}
