export interface PayPalPaymentRequest {
  amount: number;
  currency: string;
}

export interface PayPalExecuteRequest {
  paymentId: string;
  payerId: string;
}

export interface PayPalPaymentResponse {
  id: string;
  state: string;
  links: Array<{
    href: string;
    rel: string;
    method: string;
  }>;
}

export interface PayPalExecuteResponse {
  state: string;
  message?: string;
}
