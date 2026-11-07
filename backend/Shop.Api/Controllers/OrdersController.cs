using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Contracts;
using Shop.Api.Services;

namespace Shop.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public sealed class OrdersController(OrderService orders) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value, CultureInfo.InvariantCulture);

    [HttpGet]
    public Task<List<OrderSummary>> List(CancellationToken cancellationToken) =>
        orders.ListAsync(UserId, cancellationToken);

    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Checkout(CancellationToken cancellationToken)
    {
        var order = await orders.CheckoutAsync(UserId, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Get(int id, CancellationToken cancellationToken)
    {
        var order = await orders.FindAsync(UserId, id, cancellationToken);
        if (order is null)
            return NotFound();

        return Ok(order);
    }
}
