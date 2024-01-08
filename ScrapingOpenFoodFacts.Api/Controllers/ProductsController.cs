using Microsoft.AspNetCore.Mvc;
using ScrapingOpenFoodFacts.Models;

namespace ScrapingOpenFoodFacts.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MongoDBService _mongoDbService;

        public ProductsController(MongoDBService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getProduct = await _mongoDbService.GetAsync();
            return !getProduct.Any() ? NotFound("Nenhum registro cadastrado.") : Ok(getProduct);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(double code)
        {
            var getProduct = await _mongoDbService.GetAsync(code);
            return !getProduct.Any() ? NotFound(string.Format("Nenhum registro encontrado com o Codigo informado ('{0}').", code)) : Ok(getProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            await _mongoDbService.CreateAsync(product);
            var newRegister = CreatedAtAction(nameof(Get), new { id = product.Id}, product);
            return newRegister;
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Put(double code, [FromBody] Product product)
        {
            var newContent = await _mongoDbService.UpdateAsync(code, product);
            return Ok(newContent);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(double code)
        {
            await _mongoDbService.DeleteAsync(code);
            return Ok(string.Format("Registro deletado com sucesso ('{0}').", code));
        }
    }
}
