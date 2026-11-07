import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { NotificationService } from './notification.service';

interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
}

interface Session {
  token: string;
  expiresAt: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly storageKey = 'tienda.session';
  private readonly session = signal<Session | null>(this.restore());
  private timer?: ReturnType<typeof setTimeout>;

  constructor() {
    this.scheduleExpiry();
  }

  get token(): string | null {
    const session = this.session();
    return session && session.expiresAt > Date.now() ? session.token : null;
  }

  login(email: string, password: string) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, { email, password }).pipe(
      tap(response => {
        const session = {
          token: response.accessToken,
          expiresAt: Date.now() + response.expiresIn * 1000,
        };
        sessionStorage.setItem(this.storageKey, JSON.stringify(session));
        this.session.set(session);
        this.scheduleExpiry();
      }),
    );
  }

  logout(): void {
    clearTimeout(this.timer);
    this.session.set(null);
    sessionStorage.removeItem(this.storageKey);
    void this.router.navigateByUrl('/login');
  }

  expire(): void {
    if (!this.session()) return;
    this.logout();
    this.notifications.show('Tu sesión terminó', 'Vuelve a iniciar sesión para continuar.', 'info');
  }

  private scheduleExpiry(): void {
    clearTimeout(this.timer);
    const session = this.session();
    if (session) {
      this.timer = setTimeout(() => this.expire(), session.expiresAt - Date.now());
    }
  }

  private restore(): Session | null {
    try {
      const stored: unknown = JSON.parse(sessionStorage.getItem(this.storageKey) ?? 'null');
      if (stored && typeof stored === 'object' && 'token' in stored && 'expiresAt' in stored
        && typeof stored.token === 'string' && stored.token.length > 0
        && typeof stored.expiresAt === 'number' && Number.isFinite(stored.expiresAt)
        && stored.expiresAt > Date.now()) {
        return { token: stored.token, expiresAt: stored.expiresAt };
      }
    } catch {
      sessionStorage.removeItem(this.storageKey);
    }
    return null;
  }
}
