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

## Probar login y catálogo en Swagger

1. Ejecuta `dotnet run --project Shop.Api` después de preparar la base.
2. En `POST /api/auth/login`, envía el correo `demo@shop.local` y la contraseña que elegiste al cargar los datos. Una respuesta `200` contiene `accessToken`.
3. Pulsa **Authorize**, pega el token y confirma. Ejecuta `GET /api/products` con `search=TEC-001` y prueba `GET /api/products/{id}` con el ID que devuelve la búsqueda.
4. Usa un ID que no exista para comprobar el `404`. Cierra la autorización en Swagger y vuelve a consultar el catálogo para comprobar el `401`.

Los tokens duran una hora. Si no defines `Jwt__Key` en desarrollo, se genera una clave en memoria y tendrás que iniciar sesión otra vez después de reiniciar la API. Fuera de desarrollo configura `Jwt__Key` con una clave aleatoria de al menos 32 bytes y `ConnectionStrings__Shop` con la cadena de conexión del ambiente.

## Probar el carrito

1. Inicia sesión, autoriza Swagger y consulta `GET /api/products` para obtener los IDs de `TEC-001` y `TEC-002`.
2. Agrega una unidad de cada producto con `POST /api/cart/items`, enviando `productId` y `quantity`. El subtotal debe ser `100.00` y el descuento `0.00`.
3. Cambia la cantidad de `TEC-002` a dos con `PUT /api/cart/items/{productId}` y un cuerpo `{"quantity":2}`. El subtotal debe ser `130.00`, el descuento `13.00` y el total `117.00`.
4. Envía una cantidad superior al stock. Debe responder `409` y conservar la cantidad anterior.
5. Consulta `GET /api/cart`, elimina una línea con `DELETE /api/cart/items/{productId}` y vacía el resto con `DELETE /api/cart`.

Agregar una línea existente suma unidades. Actualizarla reemplaza su cantidad. El carrito consulta el stock disponible sin reservar unidades.
