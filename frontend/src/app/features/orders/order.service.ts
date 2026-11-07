import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface OrderSummary {
  id: number;
  createdAt: string;
  subtotal: number;
  discount: number;
  total: number;
}

export interface OrderLine {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface OrderResponse extends OrderSummary {
  items: OrderLine[];
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/orders`;

  checkout() {
    return this.http.post<OrderResponse>(this.url, null);
  }

  list() {
    return this.http.get<OrderSummary[]>(this.url);
  }

  get(id: number) {
    return this.http.get<OrderResponse>(`${this.url}/${id}`);
  }
}
