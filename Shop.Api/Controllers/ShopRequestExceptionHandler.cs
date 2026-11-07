using Microsoft.AspNetCore.Diagnostics;
using Shop.Api.Services;

namespace Shop.Api.Controllers;

internal sealed class ShopRequestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ShopRequestException requestError)
            return false;

        await Results.Problem(statusCode: requestError.StatusCode, title: requestError.Message)
            .ExecuteAsync(context);
        return true;
    }
}
