using System.Collections.Generic;
using System.Runtime.Serialization;
using CalNotify.Models.Addresses;

namespace CalNotify.Models.Services
{
    [DataContract]
    public class GeocodeioQueryWrapper
    {
        [DataMember(Name = "input")]
        public GeocodeioInput Input { get; set; }

        [DataMember(Name = "results")]
        public List<GeocodeioResult> Results { get; set; }
    }


    [DataContract]
    public struct GeocodeioInput
    {
        [DataMember(Name = "address_components")]
        public GeocodeiOAddressComponent AddressComponent { get; set; }

        [DataMember(Name = "formatted_address")]
        public string FormattedAddress { get; set; }
    }


    [DataContract]
    public struct GeocodeioResult : IAddress
    {
        [DataMember(Name = "address_components")]
        public GeocodeiOAddressComponent AddressComponent { get; set; }

        [DataMember(Name = "formatted_address")]
        public string FormattedAddress { get; set; }

        public string Number => AddressComponent.Number;

        public string Street => AddressComponent.Street;

        public string Suffix => AddressComponent.Suffix;

        public string City => AddressComponent.City;

        public string County => AddressComponent.County;

        public string State => AddressComponent.State;

        public string Zip => AddressComponent.Zip;


        [DataMember(Name = "location")]
        public GeocodeiOLocation GeocodeiOLocation { get; set; }

        public IGeoLocation Location => GeocodeiOLocation;

        [DataMember(Name = "accuracy")]
        public float Accuracy { get; set; }


        [DataMember(Name = "accuracy_type")]
        public string AccuracyType { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }
    }


    [DataContract]
    public struct GeocodeiOAddressComponent
    {
        [DataMember(Name = "number")]
        public string Number { get; set; }

        [DataMember(Name = "predirectional")]
        public string Predirectional { get; set; }

        [DataMember(Name = " street")]
        public string Street { get; set; }


        [DataMember(Name = "suffix")]
        public string Suffix { get; set; }

        [DataMember(Name = "formatted_street")]
        public string FormattedStreet { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "county")]
        public string County { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "zip")]
        public string Zip { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }
    }


    [DataContract]
    public struct GeocodeiOLocation : IGeoLocation
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }
}