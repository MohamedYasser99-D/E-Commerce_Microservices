using Basket.API.Entities;
using Basket.API.GrpcServices;
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
        private readonly DiscountGrpcService _discountGrpcService;
        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            _basketRepo = basketRepository;
            _discountGrpcService = discountGrpcService;
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
            foreach (var item in model.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                if(coupon != null)
                {
                    item.Price -= coupon.Amount;
                }
            }
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
