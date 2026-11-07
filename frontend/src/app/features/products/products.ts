import { CurrencyPipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { catchError, debounceTime, finalize, map, of, startWith, switchMap, tap } from 'rxjs';
import { NotificationService } from '../../core/notification.service';
import { Product, ProductService } from './product.service';
import { CartService } from '../cart/cart.service';

@Component({
  selector: 'app-products',
  imports: [CurrencyPipe, ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatProgressBarModule],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class Products {
  private readonly service = inject(ProductService);
  private readonly cart = inject(CartService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);
  readonly search = new FormControl('', { nonNullable: true });
  readonly products = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly failed = signal(false);
  readonly adding = signal<number | null>(null);

  constructor() {
    this.search.valueChanges.pipe(
      debounceTime(300),
      map(value => value.trim()),
      startWith(''),
      tap(() => { this.loading.set(true); this.failed.set(false); }),
      switchMap(value => this.service.list(value).pipe(
        map(products => ({ products, failed: false })),
        catchError(() => of({ products: [], failed: true })),
      )),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(result => {
      this.products.set(result.products);
      this.failed.set(result.failed);
      this.loading.set(false);
    });
  }

  add(product: Product): void {
    if (this.adding() !== null || product.stock === 0) return;
    this.adding.set(product.id);
    this.cart.add(product.id).pipe(finalize(() => this.adding.set(null))).subscribe({
      next: () => this.notifications.show('Producto agregado', `Agregaste ${product.name} a tu carrito.`),
      error: () => {},
    });
  }

  retry(): void {
    this.search.setValue(this.search.value);
  }
}
