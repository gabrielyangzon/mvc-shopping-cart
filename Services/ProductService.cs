using mvc_products.Models;
using System.Text.Json;

namespace mvc_products.Services
{
    public class ProductService
    {
        public ProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json");

        public IEnumerable<Product> GetProducts()
        {
            using var jsonFileReader = File.OpenText(JsonFileName);
            return JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }


        public Product GetProduct(int id)
        {
            var product = this.GetProducts().FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return null;
            }

            return product;
        }







    }
}
