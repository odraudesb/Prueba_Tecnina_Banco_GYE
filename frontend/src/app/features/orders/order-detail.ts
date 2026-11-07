import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Subject, catchError, combineLatest, map, of, startWith, switchMap, tap } from 'rxjs';
import { OrderResponse, OrderService } from './order.service';

@Component({
  selector: 'app-order-detail',
  imports: [CurrencyPipe, DatePipe, RouterLink, MatButtonModule, MatProgressBarModule],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.scss',
})
export class OrderDetail {
  private readonly service = inject(OrderService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly refresh = new Subject<void>();
  readonly order = signal<OrderResponse | null>(null);
  readonly loading = signal(true);

  constructor() {
    combineLatest([this.route.paramMap, this.refresh.pipe(startWith(undefined))]).pipe(
      tap(() => this.loading.set(true)),
      map(([params]) => Number(params.get('id'))),
      switchMap(id => Number.isInteger(id) && id > 0
        ? this.service.get(id).pipe(catchError(() => of(null)))
        : of(null)),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(order => {
      this.order.set(order);
      this.loading.set(false);
    });
  }

  retry(): void {
    this.refresh.next();
  }
}
