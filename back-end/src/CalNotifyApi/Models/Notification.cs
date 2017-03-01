using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NpgsqlTypes;

namespace CalNotifyApi.Models
{
    [DataContract]
    public class Notification
    {

        [Key]
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        [DataMember(Name = "title"),Required]
        public string Title { get; set; }

        [DataMember(Name = "details"),Required]
        public string Details { get; set; }

        [DataMember(Name = "location")]
        public PostgisPoint Location { get; set; }

        [DataMember(Name="affected_area")]
        public PostgisPolygon AffectedArea { get; set; }

    
        [DataMember(Name = "category"), Required]
        public Category Category { get; set; }

        [DataMember(Name = "source"), Required]
        public string Source { get; set; }

        [DataMember(Name = "source_id")]
        public string SourceId { get; set; }

        [DataMember(Name = "severity") , Required]
        public Severity Severity { get; set; }

        [DataMember(Name = "status")]
        public NotiStatus Status { get; set; }

        [DataMember(Name = "created")]
        public DateTime  Created { get; set; }

        [DataMember(Name = "published"), JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? Published { get; set; }


        [DataMember(Name = "author_id")]
        public Guid AuthorId { get; set; }
     
        [DataMember(Name = "author"), JsonIgnore]
        public WebAdmin Author { get; set; }

        [DataMember(Name = "published_by"), JsonIgnore]
        public WebAdmin PublishedBy { get; set; }

        [DataMember(Name = "published_by_id")]
        public Guid? PublishedById { get; set; }

    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum Category
    {
        Fire,
        Flood,
        Weather,
        Earthquake,
        Tsunami,
        Other,
        Any
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Severity
    {
        Emergency,
        NonEmergency
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotiStatus
    {
        New,
        Published,
        Archived
    }
}
