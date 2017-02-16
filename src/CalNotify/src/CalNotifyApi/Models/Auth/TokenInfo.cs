using System.Runtime.Serialization;

namespace CalNotify.Models.Auth
{
    /// <summary>
    ///     A representation holding all required tokens and other authentication related data
    ///     required for clients to interact with our api's and our other externally connected systems.
    /// </summary>
    [DataContract]
    public class TokenInfo
    {
        /// <summary>
        ///     Allows access to our API
        ///     See our documentation on authorization and Bearer tokens for more info.
        /// </summary>
        [DataMember(Name = "token")]
        public string Token { get; set; }

        /// <summary>
        ///     Our token expire time in  seconds since unix epoch.
        /// </summary>
        [DataMember(Name = "expires")]
        public long Expires { get; set; }

        /// <summary>
        ///     The user's id which this token is connected to.
        /// </summary>
        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        ///     The key to use for google maps reverse geocoding calls
        /// </summary>
        [DataMember(Name = "google_services_key")]
        public string GoogleServicesKey { get; set; }
    }
}