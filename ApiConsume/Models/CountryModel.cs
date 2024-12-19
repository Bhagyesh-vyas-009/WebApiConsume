using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace ApiConsume.Models
{
    public class CountryModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }
}
