using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Contracts;
using Shop.Api.Services;

namespace Shop.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public sealed class CartController(CartService cart) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value, CultureInfo.InvariantCulture);

    [HttpGet]
    public Task<CartResponse> Get(CancellationToken cancellationToken) =>
        cart.GetAsync(UserId, cancellationToken);

    [HttpPost("items")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<CartResponse> Add(AddCartItemRequest request, CancellationToken cancellationToken) =>
        cart.AddAsync(UserId, request.ProductId, request.Quantity, cancellationToken);

    [HttpPut("items/{productId:int}")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<CartResponse> Update(int productId, UpdateCartItemRequest request, CancellationToken cancellationToken) =>
        cart.UpdateAsync(UserId, productId, request.Quantity, cancellationToken);

    [HttpDelete("items/{productId:int}")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<CartResponse> Remove(int productId, CancellationToken cancellationToken) =>
        cart.RemoveAsync(UserId, productId, cancellationToken);

    [HttpDelete]
    public Task<CartResponse> Clear(CancellationToken cancellationToken) =>
        cart.ClearAsync(UserId, cancellationToken);
}
