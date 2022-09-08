using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using System.Net;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{userName}",Name ="GetOrder")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVM>),(int)HttpStatusCode.OK )]
        public async Task<ActionResult<IEnumerable<OrdersVM>>> GetOrdersByUserName (string userName)
        {
            var query = new GetOrderListQuery(userName);
            var orders = _mediator.Send(query);
            return Ok(orders);
        }
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand model)
        {
            var results = await _mediator.Send(model);
            return Ok(results);
        }
        [HttpPost("UpdateOrder")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand model)
        {
            await _mediator.Send(model);
            return NoContent();
        }
        [HttpDelete("{id}" , Name ="DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand() { Id = id};
            await _mediator.Send(command);
            return NoContent();
        }
       
    }
}
