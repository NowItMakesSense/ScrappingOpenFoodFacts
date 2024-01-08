using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ScrapingOpenFoodFacts.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductStatus
    {
        [BsonElement("imported")]
        [EnumMember(Value = "imported")]
        imported = 0,
        [BsonElement("draft")]
        [EnumMember(Value = "draft")]
        draft = 1,
    }
}
