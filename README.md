# Carrito de compras · Backend

API REST en ASP.NET Core 8.

## Ejecutar localmente

Desde la raíz de este repositorio, con el SDK de .NET 8 instalado:

```powershell
dotnet build Shop.Api
dotnet run --project Shop.Api
```

- Swagger UI en http://localhost:5080/swagger
- Documento OpenAPI en http://localhost:5080/swagger/v1/swagger.json
- `GET /api/health` responde con HTTP 200 y `{"status":"ok"}`

Swagger está disponible en el ambiente de desarrollo. El endpoint de salud comprueba que la API responde.
