namespace Shop.Api.Services;

internal sealed class ShopRequestException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
