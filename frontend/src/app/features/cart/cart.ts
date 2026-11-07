import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Observable, finalize } from 'rxjs';
import { CartLine, CartResponse, CartService } from './cart.service';
import { OrderService } from '../orders/order.service';
import { NotificationService } from '../../core/notification.service';
import { ConfirmDialog } from '../../shared/confirm-dialog';
import { CheckoutDialog } from './checkout-dialog';

@Component({
  selector: 'app-cart',
  imports: [CurrencyPipe, RouterLink, MatButtonModule, MatProgressBarModule],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart {
  private readonly service = inject(CartService);
  private readonly orders = inject(OrderService);
  private readonly notifications = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  readonly cart = signal<CartResponse | null>(null);
  readonly loading = signal(false);
  readonly busy = signal(false);
  readonly failed = signal(false);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.failed.set(false);
    this.service.get().pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: cart => this.cart.set(cart),
      error: () => this.failed.set(true),
    });
  }

  update(line: CartLine, quantity: number): void {
    if (this.busy()) return;
    if (!Number.isInteger(quantity) || quantity < 1 || quantity > line.stock) {
      this.notifications.show('Revisa la cantidad', 'Ingresa una cantidad válida dentro del stock disponible.', 'info');
      return;
    }
    if (quantity === line.quantity) return;
    this.change(this.service.update(line.productId, quantity), 'Cantidad actualizada', 'El total de tu carrito ya está actualizado.');
  }

  updateInput(line: CartLine, event: Event): void {
    const input = event.target as HTMLInputElement;
    this.update(line, Number(input.value));
    input.value = String(line.quantity);
  }

  remove(line: CartLine): void {
    if (this.busy()) return;
    this.change(this.service.remove(line.productId), 'Producto eliminado', `Quitaste ${line.name} de tu carrito.`);
  }

  clear(): void {
    if (this.busy()) return;
    this.dialog.open(ConfirmDialog, {
      data: { title: '¿Vaciar tu carrito?', message: 'Se quitarán todos los productos del carrito.', confirmLabel: 'Vaciar carrito' },
      width: '420px',
      maxWidth: 'calc(100vw - 32px)',
    }).afterClosed().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(confirmed => {
      if (confirmed && !this.busy()) {
        this.change(this.service.clear(), 'Carrito vacío', 'Quitaste todos los productos del carrito.');
      }
    });
  }

  checkout(): void {
    const cart = this.cart();
    if (!cart?.items.length || this.busy()) return;
    this.dialog.open(CheckoutDialog, {
      data: cart,
      width: '560px',
      maxWidth: 'calc(100vw - 32px)',
    }).afterClosed().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(confirmed => {
      if (!confirmed || this.busy()) return;
      this.busy.set(true);
      this.orders.checkout().pipe(finalize(() => this.busy.set(false))).subscribe({
        next: order => {
          void this.router.navigate(['/compras', order.id]);
          this.notifications.show('Compra realizada', `Tu compra #${order.id} quedó registrada. Puedes consultar el detalle en Mis compras.`);
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 409) this.load();
        },
      });
    });
  }

  private change(request: Observable<CartResponse>, title: string, message: string): void {
    this.busy.set(true);
    request.pipe(finalize(() => this.busy.set(false))).subscribe({
      next: cart => {
        this.cart.set(cart);
        this.notifications.show(title, message);
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 409 || error.status === 404) this.load();
      },
    });
  }
}
