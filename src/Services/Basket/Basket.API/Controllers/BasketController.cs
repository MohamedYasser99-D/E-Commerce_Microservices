using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
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
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService , IMapper mapper , IPublishEndpoint publishEndpoint)
        {
            _basketRepo = basketRepository;
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint; 
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
        
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _basketRepo.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();
            var basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            basketCheckoutEvent.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(basketCheckoutEvent);
            await _basketRepo.DeleteBasket(basket.UserName);
            return Accepted();
        }
    }
}
