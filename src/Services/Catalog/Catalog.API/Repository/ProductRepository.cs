using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Catalog.API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;
        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        }

        public async Task CreateProduct(Product product)
        {
           await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            var res = await _context.Products.DeleteOneAsync(filter);
            return res.IsAcknowledged && res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> Find(Expression<Func<Product, bool>> filters)
        {
            return await _context.Products.Find(filters).ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(p => true).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var results = await _context.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
            return results.IsAcknowledged && results.ModifiedCount > 0;
        }
    }
}
