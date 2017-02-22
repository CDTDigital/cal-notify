using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NpgsqlTypes;

namespace CalNotifyApi.Services
{
    [DataContract]
    public class ZipCodeInfo
    {
       
        [Key]
        public string Zipcode { get; set; }


        public string City { get; set; }


        public string County { get; set; }


        public string Region { get; set; }
        public PostgisPoint Location { get; set; }

        
     
    }
}