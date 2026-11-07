# Carrito de compras

El proyecto está organizado en dos carpetas:

- `backend/` contiene la API ASP.NET Core, las migraciones y los datos de prueba.
- `frontend/` contiene la aplicación Angular con Angular Material.

## Requisitos

- SDK de .NET 8.
- SQL Server en ejecución. La conexión local usa autenticación de Windows.
- Node.js 22.12 o superior dentro de la rama 22 y npm.

## Iniciar la API

Desde la raíz del repositorio:

```powershell
.\backend\iniciar.ps1
```

El script prepara la base y los datos de prueba, inicia la API en http://localhost:5080 y abre Swagger.

## Iniciar el frontend

En otra terminal:

```powershell
cd frontend
npm ci
npm start
```

La aplicación abre en http://localhost:4200. El usuario de demostración es `demo@shop.local` con contraseña `CarritoDemo2026!` si se creó con los valores predeterminados.

El frontend incluye login, catálogo, carrito, confirmación de compra e historial con detalle de cada orden. Los pasos de revisión están en [el README del frontend](frontend/README.md).

La configuración y los endpoints de la API se detallan en [el README del backend](backend/README.md).
