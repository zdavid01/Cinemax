import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreatePaymentDTO, PaymentVM } from '../types/Payment';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly baseUrl = 'http://localhost:8004';

  constructor(private http: HttpClient) {}

  getPayments(username: string): Observable<PaymentVM[]> {
    return this.http.get<PaymentVM[]>(`${this.baseUrl}/payment/get-payments/${encodeURIComponent(username)}`);
  }

  createPayment(payload: CreatePaymentDTO): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/payment/create-payment`, payload);
  }
}


