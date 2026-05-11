using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Models;
using MyFirstAPI.Services;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController()
        {
            _productService = new ProductService();
        }

        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            var products = _productService.GetProducts();

            return Ok(products);
        }
    }
}