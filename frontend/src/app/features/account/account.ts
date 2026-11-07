import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../core/auth.service';
import { NotificationService } from '../../core/notification.service';

@Component({
  selector: 'app-account',
  imports: [MatButtonModule],
  template: `
    <main class="account-page">
      <section>
        <span class="account-mark" aria-hidden="true">t</span>
        <span class="eyebrow">TU CUENTA</span>
        <h1>Bienvenido a tienda.</h1>
        <p>Has iniciado sesión correctamente.</p>
        <button mat-stroked-button (click)="logout()">Cerrar sesión</button>
      </section>
    </main>
  `,
  styles: `
    .account-page { min-height: 100dvh; display: grid; place-items: center; padding: 24px; }
    section { text-align: center; }
    .account-mark { display: grid; place-items: center; width: 60px; height: 60px;
      background: #d9efc3; color: #183f32; border-radius: 16px; font-size: 40px; font-weight: 700; margin: 0 auto 32px; }
    h1 { font-size: clamp(28px, 4vw, 42px); letter-spacing: -1px; }
    p { color: #73806b; margin-bottom: 30px; }
  `,
})
export class Account {
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);

  logout(): void {
    this.auth.logout();
    this.notifications.show('Hasta pronto', 'Cerraste sesión correctamente.', 'info');
  }
}
