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
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Events
{
    [DataContract]
    [SwaggerSchemaFilter(typeof(ExampleNotification))]
    public class CreateNotificationEvent : IValidatableObject
    {
        [DataMember(Name = "title"), Required]
        public string Title { get; set; }

        [DataMember(Name = "details"), Required]
        public string Details { get; set; }

        [DataMember(Name = "location"),]
        public GeoJsonLocation Location { get; set; }


        [DataMember(Name = "affected_area"), Required]
        public GeoPolygon AffectedArea { get; set; }


        [DataMember(Name = "category"), Required]
        public Category Category { get; set; }

        [DataMember(Name = "source"), Required]
        public string Source { get; set; }

        [DataMember(Name = "severity"), Required]
        public Severity Severity { get; set; }

        [DataMember(Name = "status")]
        public NotiStatus Status { get; set; }


        [DataMember(Name = "published")]
        public DateTime? Published { get; set; }

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
                Status = NotiStatus.New,
                Created = DateTime.Now,
                AuthorId = new Guid(id),
            };


            context.Notifications.Add(notification);
            context.SaveChanges();
            return notification;
        }

        public Notification UpdateProcess(BusinessDbContext context, long notificationId)
        {
          

            var notification = context.Notifications.FirstOrDefault(x => x.Id == notificationId);
            notification.Title = Title;
            notification.Details = Details;
            notification.Location = new PostgisPoint(Location.Coordinates[0], Location.Coordinates[1])
            {
                SRID = Constants.SRID
            };
            var affectedCoordinates = AffectedArea.Coordinates.Select(coor => new Coordinate2D(coor[0], coor[1])).ToArray();
            notification.AffectedArea = new PostgisPolygon(new[]
            {
                affectedCoordinates
            });

            notification.Category = Category;
            notification.Status = Status;
            notification.Published = Published;
            notification.Source = Source;
            notification.Severity = Severity;

           
            context.SaveChanges();
            return notification;
        }


        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Location?.Coordinates == null || Location.Coordinates.Length != 2)
            {
                yield return new ValidationResult("Location was not provided");
            }
        }
    }

    [DataContract]
    public class GeoJsonLocation
    {
        [DataMember(Name = "type")]
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


    public class ExampleNotification : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            model.Default = new CreateNotificationEvent()
            {
                Title = "Example Notification",
                Details = "Example notification details",
                Location = new GeoJsonLocation()
                {
                    Type = "Point",
                    Coordinates = new double[] {-121.51977539062499,
          38.61901643727865 }
                },
                AffectedArea = new GeoPolygon()
                {
                    Type = "Polygon",
                    Coordinates = new List<double[]>()
                    {
                        new []{ -121.3604736328125,
              39.11727568585598 },
                        new [] { -122.13226318359375,
              38.53957267203905 },
                        new [] { -120.9320068359375,
              38.14535757293734},
                        new [] {-121.3604736328125,
              39.11727568585598 }
                    }
                },
                Category = Category.Fire,
                Severity = Severity.Emergency,
                Source = "TEST"
            };
        }
    }
}
