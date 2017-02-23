using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Auth;
using Newtonsoft.Json;
using NpgsqlTypes;

namespace CalNotifyApi.Models.Addresses
{
    /// <summary>
    ///     Base class which holds the simple and most used aspects of an address.
    ///     Not exposed directly but instead is used as a base for other more complex addresses
    /// </summary>
    [DataContract]
    public abstract class SimpleAddress
    {
        [DataMember(Name = "number"),Required]
        public string Number { get; set; }

        [DataMember(Name = "street"), Required]
        public string Street { get; set; }

        [DataMember(Name = "state"), Required]
        public string State { get; set; }


        [DataMember(Name = "zip"), Required]
        public string Zip { get; set; }

        [DataMember(Name = "city"), Required]
        public string City { get; set; }
    }

    /// <summary>
    /// Small geocooridinate represenation 
    /// </summary>
    public class SimpleGeoLocation : IGeoLocation
    {
        /// <summary>
        /// Latitude position
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude position
        /// </summary>
        public double Longitude { get; set; }
    }

    /// <summary>
    /// More explicit representation of an address which is not persisted in our database, but rather,
    /// is the primary address type which gets sent down the wire to clients
    /// </summary>
    [DataContract]
    public class AddressWithLatLng : SimpleAddress, IAddress, IGeoLocation
    {
        /// <summary>
        /// Latitude position
        /// </summary>
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }


        /// <summary>
        /// Longitude position
        /// </summary>
        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        /// <summary>
        /// Data marshalling helper to convert from our PostgisPoint to a simplier <see cref="IGeoLocation"/>
        /// </summary>
        /// <returns></returns>
        public IGeoLocation Location => new SimpleGeoLocation {Latitude = Latitude, Longitude = Longitude};
    }

    /// <summary>
    ///     Our model which gets persisted to our database
    /// </summary>
    [DataContract]
    public class Address : SimpleAddress
    {
       
        public Address() {}

        /// <summary>
        /// Helper to allow use to init an address from a <see cref="AddressWithLatLng"/>
        /// </summary>
        /// <param name="addre"></param>
        /// <param name="user"></param>
        public Address(AddressWithLatLng addre, GenericUser user)
        {
            if (addre == null)
                throw new ArgumentNullException(nameof(addre));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
            Number = addre.Number;
            Street = addre.Street;
            State = addre.State;
            Zip = addre.Zip;
            City = addre.City;
            GeoLocation = new PostgisPoint(addre.Latitude, addre.Longitude) { SRID = Constants.SRID };
        }

        public Address(TempUser user)
        {
            Number = user.Number;
            Street = user.Street;
            State = user.State;
            Zip = user.Zip;
            City = user.City;
            GeoLocation = new PostgisPoint(user.Latitude, user.Longitude) { SRID = Constants.SRID };
        }

        /// <summary>
        /// The id to use.
        /// If not set, it will be null added and saved in the database
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// A complete string representation of the address
        /// <remarks>Not used</remarks>
        /// TODO: fill this out when we create an address
        /// </summary>
        [DataMember(Name = "formatted_address")]
        public string FormattedAddress { get; set; }

        /// <summary>
        ///     Gets or Sets GeoLocation
        /// </summary>
        [DataMember(Name = "location")]
        public PostgisPoint GeoLocation { get; set; }

        /// <summary>
        /// The User which used this address
        /// NOTE: Will be null when queried from the database, 
        /// unless an explcit call to .Include(a=> a.User) is made 
        /// </summary>
        [DataMember(Name = "user")]
        [ForeignKey("UserId")]
        [JsonIgnore]
        public GenericUser User { get; set; }

        /// <summary>
        ///    The id of the customer that used this address
        /// </summary>
        [DataMember(Name = "user_id")]
        [JsonIgnore]
        internal Guid UserId { get; set; }

       

    }
}