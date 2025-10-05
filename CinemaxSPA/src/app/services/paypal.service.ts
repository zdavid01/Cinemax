import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PayPalPaymentRequest, PayPalExecuteRequest, PayPalPaymentResponse, PayPalExecuteResponse } from '../types/PayPal';

@Injectable({
  providedIn: 'root'
})
export class PayPalService {
  private apiUrl = 'http://localhost:8004/api/paypal'; // PayPal API URL

  constructor(private http: HttpClient) { }

  createPayment(request: PayPalPaymentRequest): Observable<PayPalPaymentResponse> {
    return this.http.post<PayPalPaymentResponse>(`${this.apiUrl}/create-payment`, request);
  }

  executePayment(request: PayPalExecuteRequest): Observable<PayPalExecuteResponse> {
    return this.http.post<PayPalExecuteResponse>(`${this.apiUrl}/execute-payment`, request);
  }

  returnFromPayPal(paymentId: string, payerId: string): Observable<PayPalExecuteResponse> {
    return this.http.get<PayPalExecuteResponse>(`${this.apiUrl}/return?paymentId=${paymentId}&payerId=${payerId}`);
  }
}
