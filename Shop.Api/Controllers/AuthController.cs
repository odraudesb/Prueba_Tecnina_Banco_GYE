using Microsoft.AspNetCore.Mvc;
using Shop.Api.Contracts;
using Shop.Api.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LoginAsync(request, cancellationToken);
        if (result is null)
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Credenciales incorrectas.");

        return Ok(result);
    }
}
