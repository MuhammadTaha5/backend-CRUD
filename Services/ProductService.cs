using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class ProductService
    {
        public List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 3000
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse",
                    Price = 2000
                }

            };
        }
    }
}