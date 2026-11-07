namespace Shop.Api.Contracts;

public sealed record ProductResponse(int Id, string Code, string Name, string Category, decimal Price, int Stock);
