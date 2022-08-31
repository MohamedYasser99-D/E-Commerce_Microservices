using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [Route("Api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepo;
        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepo = basketRepository;
        }
        [HttpGet("{userName}",Name ="GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart),(int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _basketRepo.GetBasket(username);
            return Ok(basket?? new ShoppingCart(username));
        }
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> updateBasket([FromBody] ShoppingCart model )
        {
            var basket = await _basketRepo.UpdateBasket(model);
            return Ok(basket);
        }
        [HttpDelete("{username}",Name ="DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket (string username)
        {
            await _basketRepo.DeleteBasket(username);
            return Ok();
        }
       
    }
}
