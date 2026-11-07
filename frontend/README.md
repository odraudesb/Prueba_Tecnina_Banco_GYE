# Tienda · Frontend

Aplicación Angular 21 con Angular Material.

## Ejecutar

Requiere Node.js 22.12 o superior dentro de la rama 22, npm y el backend en http://localhost:5080.

```powershell
npm ci
npm start
```

El navegador abre http://localhost:4200. La dirección de la API está en `src/environments/environment.ts`.

En la carpeta `backend`, `.\iniciar.ps1` prepara la base y levanta la API. El usuario local de demostración es `demo@shop.local` con contraseña `CarritoDemo2026!`, si se creó con los valores predeterminados.

## Revisar el login

1. Ingresa un correo inválido y revisa el mensaje bajo el campo.
2. Usa una contraseña incorrecta. Aparece un diálogo que explica el error.
3. Inicia sesión con el usuario demo. Aparece la bienvenida y se abre el catálogo.
4. Recarga la página. La sesión se conserva en la misma pestaña.
5. Cierra sesión. Aparece una confirmación y regresas al formulario.
6. Intenta entrar a `/productos` después de cerrar sesión. Debe regresar al login.

## Revisar el catálogo

Busca un producto por nombre, código o categoría. La pantalla muestra el precio y el stock que devuelve la API. Una búsqueda sin coincidencias muestra un mensaje. Si la API no responde, puedes reintentar la carga.

## Revisar el carrito

1. Agrega un producto desde el catálogo y abre **Mi carrito**.
2. Cambia la cantidad con los botones o el campo numérico. El total se actualiza con la respuesta de la API.
3. Con un teclado y un mouse, el subtotal es $100 y el descuento es $0. Con un teclado y dos mouse, el subtotal es $130, el descuento $13 y el total $117.
4. Revisa los mensajes de stock insuficiente. Puedes quitar productos o vaciar el carrito, previa confirmación.

El token se conserva en sessionStorage y se elimina al cerrar sesión o vencer. La API autoriza cada solicitud. Fuera del ambiente local, el frontend y el backend requieren HTTPS.

Para generar la versión de producción, ejecuta `npm run build`.
