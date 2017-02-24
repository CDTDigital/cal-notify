using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models;
using CalNotifyApi.Services;
using NpgsqlTypes;

namespace CalNotifyApi.Events
{
    [DataContract]
    public class CreateNotificationEvent: IValidatableObject
    {
        [DataMember(Name = "title"), Required]
        public string Title { get; set; }

        [DataMember(Name = "details"), Required]
        public string Details { get; set; }

        [DataMember(Name = "location"), Required]
        public GeoJsonLocation Location { get; set; }


        [DataMember(Name = "affected_area"), Required]
        public GeoPolygon AffectedArea { get; set; }


        [DataMember(Name = "category"), Required]
        public Category Category { get; set; }

        [DataMember(Name = "source"), Required]
        public string Source { get; set; }

        [DataMember(Name = "severity"), Required]
        public Severity Severity { get; set; }

      

        public Notification Process(BusinessDbContext context, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
             throw new ProcessEventException("Could not get the id of the author creating the request");   
            }


            var affectedCoordinates = AffectedArea.Coordinates.Select(coor => new Coordinate2D(coor[0], coor[1])).ToArray();
           var notification = new Notification()
           {
               Title = Title,
               Details = Details,
               Location = new PostgisPoint(Location.Coordinates[0], Location.Coordinates[1]) { SRID = Constants.SRID },
               AffectedArea = new PostgisPolygon(new[]
               {
                   affectedCoordinates
               }),
               Category = Category,
               Source = Source,
               Severity = Severity,
               Status =  NotiStatus.New,
               Created = DateTime.Now,
               AuthorId = new Guid(id),
           };


            context.Notifications.Add(notification);
            context.SaveChanges();
            return notification;
        }


        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Location == null || Location.Coordinates == null || Location.Coordinates.Length != 2)
            {
                yield return  new ValidationResult("Location was not provided");
            }
        }
    }

    [DataContract]
    public class GeoJsonLocation
    {
        [DataMember(Name ="type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public double[] Coordinates { get; set; }
    }

    [DataContract]
    public class GeoPolygon
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public List<double[]> Coordinates { get; set; }
    }
}
