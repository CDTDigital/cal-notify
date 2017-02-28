using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalNotifyApi.Utils
{
 
    public class NullableDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return DateTime.MinValue;

            return (DateTime)reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTimeValue = (DateTime)value;
            if (dateTimeValue == DateTime.MinValue)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value);
        }
    }
}
