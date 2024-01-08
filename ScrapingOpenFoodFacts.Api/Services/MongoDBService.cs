using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ScrapingOpenFoodFacts.Models;

namespace ScrapingOpenFoodFacts.Api
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Product> _productColletion;
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DataBaseName);

            _productColletion = database.GetCollection<Product>(mongoDBSettings.Value.ColletionName);
        }

        public async Task CreateAsync(Product product)
        {
            product.status = ProductStatus.imported;
            product.imported_t = DateTimeOffset.Now;

            await _productColletion.InsertOneAsync(product);
            return;
        }

        public async Task<List<Product>> GetAsync()
        {
            var getProduct = await _productColletion.Find(new BsonDocument()).ToListAsync();
            return getProduct;
        }

        public async Task<List<Product>> GetAsync(double code)
        {
            return await _productColletion.Find(item => item.code == code).ToListAsync();
        }

        public async Task<Product> UpdateAsync(double code, Product product)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("code", code);
            UpdateDefinition<Product> update = Builders<Product>.Update.CurrentDate("lastModified");
            return await _productColletion.FindOneAndUpdateAsync(filter, update);
        }

        public async Task DeleteAsync(double code)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("code", code);
            await _productColletion.DeleteOneAsync(filter);
            return;
        }
    }
}
