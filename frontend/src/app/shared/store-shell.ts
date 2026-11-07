import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../core/auth.service';
import { NotificationService } from '../core/notification.service';

@Component({
  selector: 'app-store-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatButtonModule],
  template: `
    <header class="site-header">
      <a class="brand" routerLink="/productos" aria-label="Tienda, catálogo"><span>t</span>tienda.</a>
      <nav aria-label="Menú principal">
        <a routerLink="/productos" routerLinkActive="active" mat-button>Catálogo</a>
      </nav>
      <button mat-stroked-button (click)="logout()">Cerrar sesión</button>
    </header>
    <router-outlet />
    <footer class="site-footer">tienda. <span>Encuentra algo que te guste.</span></footer>
  `,
  styles: `
    .site-header { display: flex; align-items: center; justify-content: space-between; gap: 18px;
      padding: 20px max(24px, calc((100vw - 1160px) / 2)); background: #fff; border-bottom: 1px solid #e3e8df; }
    .brand { color: #183f32; display: inline-flex; align-items: center; gap: 10px; text-decoration: none;
      font-weight: 750; font-size: 25px; letter-spacing: -1px; }
    .brand span { background: #d9efc3; border-radius: 9px; padding: 3px 12px; }
    nav { display: flex; gap: 4px; }
    nav .active { background: #e8f0e1; color: #183f32; }
    .site-footer { border-top: 1px solid #e3e8df; padding: 25px; text-align: center; font-weight: 700; color: #3c5545; }
    .site-footer span { font-weight: 400; font-size: 12px; margin-left: 15px; color: #7a8577; }
    @media (max-width: 720px) {
      .site-header { flex-wrap: wrap; padding: 18px 20px 12px; }
      nav { order: 3; width: 100%; justify-content: center; border-top: 1px solid #e3e8df; padding-top: 10px; }
    }
  `,
})
export class StoreShell {
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);

  logout(): void {
    this.auth.logout();
    this.notifications.show('Hasta pronto', 'Cerraste sesión correctamente.', 'info');
  }
}
