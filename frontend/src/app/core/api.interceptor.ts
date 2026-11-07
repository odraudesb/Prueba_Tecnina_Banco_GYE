import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { NotificationService } from './notification.service';

export const apiInterceptor: HttpInterceptorFn = (request, next) => {
  if (!request.url.startsWith(`${environment.apiUrl}/`)) return next(request);

  const auth = inject(AuthService);
  const notifications = inject(NotificationService);
  const isLogin = request.url === `${environment.apiUrl}/auth/login`;
  const token = auth.token;
  const authenticated = token && !isLogin
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authenticated).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isLogin) {
        auth.expire();
      } else {
        const isCheckout = request.method === 'POST' && request.url === `${environment.apiUrl}/orders`;
        const messages: Record<number, string> = {
          0: isCheckout
            ? 'Se perdió la conexión y no pudimos confirmar el resultado. Revisa Mis compras antes de volver a comprar.'
            : 'No pudimos conectar con la tienda. Revisa tu conexión e inténtalo de nuevo.',
          400: 'Revisa los datos e inténtalo de nuevo.',
          401: 'El correo o la contraseña son incorrectos.',
          403: 'Tu cuenta no tiene permiso para realizar esta acción.',
          404: 'No encontramos lo que buscas. Actualiza la página e inténtalo de nuevo.',
          409: 'La información cambió. Revisa los datos antes de continuar.',
        };
        if (error.status === 409) {
          messages[409] = error.error?.title === 'El carrito está vacío.'
            ? 'Tu carrito está vacío. Agrega un producto antes de comprar.'
            : 'No hay suficientes unidades para completar la acción. Revisa las cantidades y el stock disponible.';
        }
        notifications.show(
          'No pudimos completar la acción',
          messages[error.status] ?? 'Ocurrió un problema. Inténtalo de nuevo en unos momentos.',
          'error',
        );
      }
      return throwError(() => error);
    }),
  );
};
