using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infraestructure.Data;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Catalog.Infraestructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        public ICatalogContext _context { get; }

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        async Task<Pagination<Product>> IProductRepository.GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter &= builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(p => p.Brands.Id, catalogSpecParams.BrandId);
            }

            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(p => p.Types.Id, catalogSpecParams.TypeId);
            }

            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogSpecParams, filter);
            //var data = await _context.Products
            //    .Find(filter)
            //    .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
            //    .Limit(catalogSpecParams.PageSize)
            //    .ToListAsync();

            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                (int)totalItems,
                data);
        }

        async Task<Product> IProductRepository.GetProduct(string id)
        {
            return await _context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductsByName(string name)
        {
            return await _context.Products
                .Find(p => p.Name.ToLower() == name.ToLower())
                .ToListAsync();
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductsByBrand(string brandName)
        {
            return await _context.Products
                .Find(p => p.Brands.Name.ToLower() == brandName.ToLower())
                .ToListAsync();
        }

        async Task<Product> IProductRepository.CreateProduct(Product product)
        {
            await _context.Products
                .InsertOneAsync(product);

            return product;
        }

        async Task<bool> IProductRepository.UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products
                .ReplaceOneAsync(p => p.Id == product.Id, product);

            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        async Task<bool> IProductRepository.DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products
                .DeleteOneAsync(p => p.Id == id);

            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        async Task<IEnumerable<ProductBrand>> IBrandRepository.GetAllBrands()
        {
            return await _context.Brands
                .Find(b => true)
                .ToListAsync();
        }

        async Task<IEnumerable<ProductType>> ITypesRepository.GetAllTypes()
        {
            return await _context.Types
                .Find(t => true)
                .ToListAsync();
        }

        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefinition = Builders<Product>.Sort.Ascending("Name"); // Default
            
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceAsc":
                        sortDefinition = Builders<Product>.Sort.Ascending(p => p.Price);
                        break;
                    case "priceDesc":
                        sortDefinition = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        sortDefinition = Builders<Product>.Sort.Ascending(p => p.Name);
                        break;
                }
            }

            return await _context.Products
                .Find(filter)
                .Sort(sortDefinition)
                .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();
        }
    }
}
