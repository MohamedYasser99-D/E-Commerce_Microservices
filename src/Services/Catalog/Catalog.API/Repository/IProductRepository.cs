using Catalog.API.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Catalog.API.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductById(string id);
        Task<IEnumerable<Product>> Find(Expression<Func<Product,bool>> filters);
        Task CreateProduct (Product product);  
        Task<bool> UpdateProduct (Product product);
        Task<bool> DeleteProduct (string id);

    }
}
