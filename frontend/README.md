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
3. Inicia sesión con el usuario demo. Aparece la bienvenida y se abre la pantalla de tu cuenta.
4. Recarga la página. La sesión se conserva en la misma pestaña.
5. Cierra sesión. Aparece una confirmación y regresas al formulario.
6. Intenta entrar a `/inicio` después de cerrar sesión. Debe regresar al login.

El token se conserva en sessionStorage y se elimina al cerrar sesión o vencer. La API autoriza cada solicitud. Fuera del ambiente local, el frontend y el backend requieren HTTPS.

Para generar la versión de producción, ejecuta `npm run build`.
