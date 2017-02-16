using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CalNotify.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public  class GeoLocation : IEquatable<GeoLocation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoLocation" /> class.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        public GeoLocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;

        }


        /// <summary>
        /// Gets or Sets Latitude
        /// </summary>
        [DataMember(Name = "lat"), Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or Sets Longitude
        /// </summary>
        [DataMember(Name = "lng"), Required]
        public double Longitude { get; set; }

        #region Operators

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GeoLocation)obj);
        }

        /// <summary>
        /// Returns true if GeoLocation instances are equal
        /// </summary>
        /// <param name="other">Instance of GeoLocation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GeoLocation other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    this.Latitude == other.Latitude ||
                    this.Latitude.Equals(other.Latitude)
                ) &&
                (
                    this.Longitude == other.Longitude ||
                    this.Longitude.Equals(other.Longitude)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                
                    hash = hash * 59 + this.Latitude.GetHashCode();
               
                    hash = hash * 59 + this.Longitude.GetHashCode();
                return hash;
            }
        }



        public static bool operator ==(GeoLocation left, GeoLocation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GeoLocation left, GeoLocation right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
