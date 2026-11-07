# Carrito de compras · Backend

API REST en ASP.NET Core 8.

## Ejecutar localmente

Desde la raíz de este repositorio, con el SDK de .NET 8 instalado:

```powershell
dotnet build Shop.Api
dotnet run --project Shop.Api
```

- El perfil local abre Swagger en el navegador predeterminado. También puedes abrir http://localhost:5080/ para ir a Swagger.
- Documento OpenAPI en http://localhost:5080/swagger/v1/swagger.json
- `GET /api/health` responde con HTTP 200 y `{"status":"ok"}`

Swagger está disponible en el ambiente de desarrollo. El endpoint de salud comprueba que la API responde.

## Preparar la base de datos

En desarrollo la API usa SQL Server en `localhost`, autenticación de Windows y una base dedicada llamada `PruebaTecnicaCarrito`. La cuenta debe tener permiso para crear la base o aplicar cambios de esquema si ya existe. Si usas otra instancia, configura `ConnectionStrings__Shop` en tu terminal. La configuración de desarrollo confía en el certificado del servidor local.

Desde la raíz del repositorio, aplica la migración y carga los datos de prueba:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet tool restore
dotnet ef database update --project Shop.Api

$password = Read-Host 'Contraseña de prueba (mínimo 12 caracteres)' -AsSecureString
$env:Demo__Password = [System.Net.NetworkCredential]::new('', $password).Password
dotnet run --project Shop.Api -- --seed-demo
Remove-Item Env:Demo__Password
```

La carga crea el usuario `demo@shop.local` y tres productos de ejemplo. La base guarda el hash de la contraseña. Si repites el comando, se conservan la contraseña y el stock que ya estén guardados. El comando de carga termina al guardar los datos. Para iniciar la API, ejecuta `dotnet run --project Shop.Api`.

Si tienes `sqlcmd`, puedes comprobar los datos en la instancia local:

```powershell
sqlcmd -S localhost -E -C -d PruebaTecnicaCarrito -Q "SELECT Email FROM dbo.Users; SELECT Code, Name, Price, Stock FROM dbo.Products;"
```
