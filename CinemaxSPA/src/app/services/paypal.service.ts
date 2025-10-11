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
    // The authentication interceptor will automatically add JWT token to this request
    return this.http.post<PayPalPaymentResponse>(`${this.apiUrl}/create-payment`, request);
  }

  executePayment(request: PayPalExecuteRequest): Observable<PayPalExecuteResponse> {
    // The authentication interceptor will automatically add JWT token to this request
    return this.http.post<PayPalExecuteResponse>(`${this.apiUrl}/execute-payment`, request);
  }

  returnFromPayPal(paymentId: string, payerId: string): Observable<PayPalExecuteResponse> {
    // This endpoint doesn't require authentication as it's called from PayPal's redirect
    return this.http.get<PayPalExecuteResponse>(`${this.apiUrl}/return?paymentId=${paymentId}&payerId=${payerId}`);
  }

  // Test endpoint that doesn't require authentication (for development/testing)
  testExecutePayment(request: PayPalExecuteRequest): Observable<PayPalExecuteResponse> {
    return this.http.post<PayPalExecuteResponse>(`${this.apiUrl}/test-execute-payment`, request);
  }
}
