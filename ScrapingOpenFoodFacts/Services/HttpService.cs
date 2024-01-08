using Newtonsoft.Json;
using ScrapingOpenFoodFacts.Models;
using System.Net;
using System.Text;

namespace ScrapingOpenFoodFacts
{
    public class HttpService : IDisposable
    {
        private HttpClient _client = null!;
        private readonly string _baseUrl = "https://localhost:7189";

        private JsonSerializerSettings jsonSettings()
        {
            var options = new JsonSerializerSettings();
            options.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            options.NullValueHandling = NullValueHandling.Ignore;
            return options;
        }

        public async Task<List<Product>?> Get(string endPoint)
        {
            try
            {
                _client = new HttpClient();

                var url = new Uri($"{_baseUrl}{endPoint}");
                var responce = await _client.GetAsync(url).ConfigureAwait(false);
                var responceContent = responce.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (responce.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<List<Product>>(responceContent, jsonSettings())!;
            }
            catch (Exception)
            {
                Console.WriteLine($"Erro ao consultar pedido (EndPoint : '{endPoint}').");
                throw;
            }
        }

        public async Task<Product?> Post(string endPoint, Product product)
        {
            try
            {
                _client = new HttpClient();

                var url = new Uri($"{_baseUrl}{endPoint}");
                var json = JsonConvert.SerializeObject(product, jsonSettings());
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var responce = await _client.PostAsync(url, data).ConfigureAwait(false);
                var responceContent = responce.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (responce.StatusCode != HttpStatusCode.Created)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<Product>(responceContent, jsonSettings());
            }
            catch (Exception)
            {
                Console.WriteLine($"Erro ao cadastrar produto (EndPoint : '{endPoint}').");
                throw;
            }
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
