using HtmlAgilityPack;
using ScrapingOpenFoodFacts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScrapingOpenFoodFacts
{
    public class ScrappingHtml(string url)
    {
        #region Private Methods
        private async Task<HtmlDocument> GetDocument()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(url);
            return doc;
        }

        private async Task<HtmlDocument> GetCustomDocument(string newUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(newUrl);
            return doc;
        }

        private async Task<List<string>> GetFoodListAsync()
        {
            List<string> foodList = new List<string>();
            HtmlDocument doc = await GetDocument();
            Uri baseUri = new Uri(url);

            var linkNodes = doc.DocumentNode.SelectNodes("//ul[@class='products']/li")?.ToList();
            linkNodes?.ForEach(link =>
            {
                string? href = link.SelectSingleNode(".//a")?.Attributes["href"].Value;
                foodList.Add(new Uri(baseUri, href).AbsoluteUri);
            });

            return foodList;
        }

        private async Task<Product?> GetCurrentProductAsync(string productUrl)
        {
            double productCode;

            try
            {
                var loadUrl = await GetCustomDocument(productUrl);
                var htmlProduct = loadUrl.DocumentNode.SelectNodes("//div[@class='card-section']/div")[0];

                var product = new Product();

                double.TryParse(GetCode(htmlProduct), out productCode);

                product.Id = "";
                product.code = productCode;
                product.product_name = GetName(htmlProduct);
                product.barcode = $"{productCode} {GetBarCodeUnit(htmlProduct)}";
                product.quantity = GetQuantity(htmlProduct);
                product.categories = GetCategories(htmlProduct);
                product.brands = GetBrands(htmlProduct);
                product.packaging = GetPackaging(htmlProduct);
                product.image_url = GetImageUrl(htmlProduct)!;
                product.url = productUrl;
                product.status = ProductStatus.draft;


                return product;
            }
            catch (Exception)
            {
                Console.WriteLine($"Erro ao carregar informações do produto ('{productUrl}')");
                throw;
            }
        }

        private async Task RegisterProducts(string? productUrl)
        {
            try
            {
                Console.WriteLine($"Recuperando informações do produto : '{productUrl}'");
                var product = await GetCurrentProductAsync(productUrl!);

                var httpService = new HttpService();
                var endPoint = $"/api/products/";

                Console.WriteLine($"Consumindo API para cadastro do produto : '{product?.product_name}' ({product?.code})");
                var response = await httpService.Get($"{endPoint}{product?.code}");
                if (response is null)
                {
                    await httpService.Post(endPoint, product!);
                    Console.WriteLine($"Produto Cadastro com Sucesso!! ({product?.code})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        private string? GetName(HtmlNode node)
        {
            return node.SelectSingleNode("//h2[@property='food:name']")?.FirstChild?.InnerText;
        }

        private string? GetCode(HtmlNode node)
        {
            return node.SelectSingleNode(".//span[@id='barcode']")?.FirstChild?.InnerText;
        }

        private string? GetBarCodeUnit(HtmlNode node)
        {
            return node.SelectSingleNode("//p[@id='barcode_paragraph']")?.LastChild?.InnerText;
        }

        private string? GetQuantity(HtmlNode node)
        {
            return node.SelectSingleNode("//span[@id='field_quantity_value']")?.FirstChild?.InnerText;
        }

        private string? GetCategories(HtmlNode node)
        {
            var linkNodes = node.SelectNodes("//span[@id='field_categories_value']/a")?.ToList();
            List<string> categoriesList = new List<string>();

            linkNodes?.ForEach(link => categoriesList.Add(link.InnerText));

            return string.Join(", ", categoriesList.ToArray());
        }

        private string? GetBrands(HtmlNode node)
        {
            var linkNodes = node.SelectNodes("//span[@id='field_brands_value']/a")?.ToList();
            List<string> categoriesList = new List<string>();

            linkNodes?.ForEach(link => categoriesList.Add(link.InnerText));

            return string.Join(", ", categoriesList.ToArray());
        }

        private string? GetPackaging(HtmlNode node)
        {
            var linkNodes = node.SelectNodes("//span[@id='field_packaging_value']/a")?.ToList();
            List<string> categoriesList = new List<string>();

            linkNodes?.ForEach(link => categoriesList.Add(link.InnerText));

            return string.Join(", ", categoriesList.ToArray());
        }

        private string? GetImageUrl(HtmlNode node)
        {
            var element = node.SelectSingleNode("//img[@id='og_image']");
            return element?.Attributes["src"]?.Value;
        }
        #endregion

        public async Task RunJobAsync()
        {
            try
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine($" Job Iniciado : {DateTimeOffset.Now}");
                Console.WriteLine("------------------------------------------------------------");

                var foodList = await GetFoodListAsync();
                await Task.WhenAll(foodList.Select(urls => RegisterProducts(urls)));

                Console.WriteLine($" Job Finalizado : {DateTimeOffset.Now}");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
