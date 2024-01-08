using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ScrapingOpenFoodFacts.Models
{
    [Serializable]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public double code { get; set; }
        public string? product_name { get; set; }
        public string? quantity { get; set; }
        public string? barcode { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public ProductStatus? status { get; set; }
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? imported_t { get; set; }
        public DateTimeOffset? lastModified { get; set; }
        public string? url { get; set; }
        public string? categories { get; set; }
        public string? packaging { get; set; }
        public string? brands { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string? image_url { get; set; }
    }
}
