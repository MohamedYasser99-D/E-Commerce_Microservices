using Catalog.API.Entities;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
      //  private readonly ILogger _logger;
        public CatalogController(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct ()
        {
            return Ok( await _productRepository.GetProducts());
        }

        [HttpGet("{id:length(24)}",Name ="GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
                //_logger.LogError($"Product with id:{id} not found");
            }
            return Ok(product);
        }

        [HttpGet]
        [Route("GetProductByName/{name}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductByName (string name)
        {
            Expression<Func<Product,bool>> expression = product => product.Name == name;    
            var product = await _productRepository.Find(expression);
            if (product == null)
            {
                return NotFound();
                //_logger.LogError($"Product with Name:{name} not found");
            }
            return Ok(product);

        }


        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody]Product model)
        {
            await _productRepository.CreateProduct(model);
            return CreatedAtRoute("GetProduct", new { id = model.Id }, model);
        }


        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody]Product model)
        {
            await _productRepository.UpdateProduct(model);
            return CreatedAtRoute("GetProduct", new { id = model.Id }, model);
        }


        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));

        }
    }
}
