import { CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CartResponse } from './cart.service';

@Component({
  selector: 'app-checkout-dialog',
  imports: [CurrencyPipe, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Confirma tu compra</h2>
    <mat-dialog-content>
      <p>Revisa los productos y el total antes de continuar.</p>
      <ul>
        @for (item of cart.items; track item.productId) {
          <li><span>{{ item.name }} × {{ item.quantity }}</span><strong>{{ item.subtotal | currency:'USD' }}</strong></li>
        }
      </ul>
      <div class="amount"><span>Subtotal</span><span>{{ cart.subtotal | currency:'USD' }}</span></div>
      <div class="amount"><span>Descuento</span><span>−{{ cart.discount | currency:'USD' }}</span></div>
      <div class="amount total"><strong>Total de la compra</strong><strong>{{ cart.total | currency:'USD' }}</strong></div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button [mat-dialog-close]="false" cdkFocusInitial>Seguir revisando</button>
      <button mat-flat-button [mat-dialog-close]="true">Confirmar compra</button>
    </mat-dialog-actions>
  `,
  styles: `
    ul { list-style: none; padding: 0; margin: 24px 0; border-bottom: 1px solid #dfe6dc; }
    li { display: flex; gap: 20px; justify-content: space-between; margin: 18px 0; line-height: 1.5; }
    li strong { white-space: nowrap; }
    .amount { display: flex; gap: 20px; justify-content: space-between; margin: 12px 0; }
    .total { margin-top: 20px; color: #285741; font-size: 18px; }
  `,
})
export class CheckoutDialog {
  readonly cart = inject<CartResponse>(MAT_DIALOG_DATA);
}
