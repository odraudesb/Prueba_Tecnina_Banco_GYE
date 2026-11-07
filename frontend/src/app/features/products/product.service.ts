import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface Product {
  id: number;
  code: string;
  name: string;
  category: string;
  price: number;
  stock: number;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);

  list(search: string) {
    const params = new HttpParams().set('search', search);
    return this.http.get<Product[]>(`${environment.apiUrl}/products`, { params });
  }
}
