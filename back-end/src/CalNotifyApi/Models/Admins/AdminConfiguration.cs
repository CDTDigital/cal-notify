using System.Runtime.Serialization;

namespace CalNotifyApi.Models.Admins
{
    /// <summary>
    /// The configurable properties of our system which need to be persisted to the database.
    /// NOTE: Only one row will ever be created of this datatype.
    /// </summary>
    [DataContract]
    public class AdminConfiguration
    {
        /// <summary>
        ///    Row id.
        /// NOTE: Will only ever be "1"
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }


    }
}