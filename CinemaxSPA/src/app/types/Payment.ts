export interface PaymentItemVM {
  id: number;
  movieName: string;
  movieId: string;
  price: number;
  quantity: number;
}

export interface PaymentVM {
  id: number;
  amount: number;
  currency: string;
  buyerId: string;
  buyerUsername: string;
  totalPrice: number;
  paymentItems: PaymentItemVM[];
}

export interface CreatePaymentItemDTO {
  movieName: string;
  movieId: string;
  price: number;
  quantity: number;
}

export interface CreatePaymentDTO {
  buyerId: string;
  buyerUsername: string;
  currency: string;
  paymentItems: CreatePaymentItemDTO[];
}

