using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Contracts;
using Shop.Api.Services;

namespace Shop.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public sealed class ProductsController(ProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll(
        [FromQuery, StringLength(120)] string? search,
        [FromQuery, StringLength(80)] string? category,
        CancellationToken cancellationToken) =>
        Ok(await products.ListAsync(search, category, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Get(int id, CancellationToken cancellationToken)
    {
        var product = await products.FindAsync(id, cancellationToken);
        if (product is null)
            return NotFound();

        return Ok(product);
    }
}
