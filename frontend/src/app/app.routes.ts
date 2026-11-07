import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth.guard';
import { StoreShell } from './shared/store-shell';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/login/login').then(module => module.Login),
  },
  {
    path: '',
    component: StoreShell,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: 'productos',
        loadComponent: () => import('./features/products/products').then(module => module.Products),
      },
      { path: 'inicio', redirectTo: 'productos', pathMatch: 'full' },
      { path: '', redirectTo: 'productos', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'productos' },
];
