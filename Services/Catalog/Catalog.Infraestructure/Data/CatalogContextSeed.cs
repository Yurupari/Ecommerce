using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infraestructure.Data
{
    public static class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool checkProducts = productCollection.Find(p => true).Any();

            if (!checkProducts)
            {
                string path = Path.Combine("Data", "SeedData", "products.json");
                var productsData = File.ReadAllText(path);
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products != null)
                {
                    foreach (var item in products)
                    {
                        productCollection.InsertOneAsync(item);
                    }
                }
            }
        }
    }
}
