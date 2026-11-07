import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface CartLine {
  productId: number;
  code: string;
  name: string;
  unitPrice: number;
  quantity: number;
  stock: number;
  subtotal: number;
}

export interface CartResponse {
  items: CartLine[];
  subtotal: number;
  discount: number;
  total: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/cart`;

  get() {
    return this.http.get<CartResponse>(this.url);
  }

  add(productId: number, quantity = 1) {
    return this.http.post<CartResponse>(`${this.url}/items`, { productId, quantity });
  }

  update(productId: number, quantity: number) {
    return this.http.put<CartResponse>(`${this.url}/items/${productId}`, { quantity });
  }

  remove(productId: number) {
    return this.http.delete<CartResponse>(`${this.url}/items/${productId}`);
  }

  clear() {
    return this.http.delete<CartResponse>(this.url);
  }
}
