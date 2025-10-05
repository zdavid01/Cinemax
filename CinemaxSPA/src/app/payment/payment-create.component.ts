import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../services/payment.service';

@Component({
  selector: 'app-payment-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container">
    <h2>Create Payment</h2>
    <form (ngSubmit)="submit()">
      <label>Buyer Id: <input [ngModel]="buyerId()" (ngModelChange)="buyerId.set($event)" name="buyerId" required /></label>
      <label>Buyer Username: <input [ngModel]="buyerUsername()" (ngModelChange)="buyerUsername.set($event)" name="buyerUsername" required /></label>
      <label>Currency: <input [ngModel]="currency()" (ngModelChange)="currency.set($event)" name="currency" required /></label>
      <fieldset>
        <legend>Item</legend>
        <label>Movie Name: <input [ngModel]="movieName()" (ngModelChange)="movieName.set($event)" name="movieName" required /></label>
        <label>Movie Id: <input [ngModel]="movieId()" (ngModelChange)="movieId.set($event)" name="movieId" required /></label>
        <label>Price: <input type="number" step="0.01" [ngModel]="price()" (ngModelChange)="price.set($event)" name="price" required /></label>
        <label>Quantity: <input type="number" [ngModel]="quantity()" (ngModelChange)="quantity.set($event)" name="quantity" required /></label>
      </fieldset>
      <button type="submit">Create</button>
    </form>
    <p *ngIf="createdId()">Created payment with id {{ createdId() }}</p>
  </div>
  `,
  styles: [`
    .container { padding: 1rem; }
    form { display: grid; gap: .5rem; max-width: 480px }
    fieldset { border: 1px solid #ddd; padding: .5rem; }
    label { display: grid; gap: .25rem }
  `]
})
export class PaymentCreateComponent {
  private readonly api = inject(PaymentService);

  buyerId = signal('testuser3');
  buyerUsername = signal('testuser3');
  currency = signal('USD');
  movieName = signal('Demo Movie');
  movieId = signal('M999');
  price = signal(9.99);
  quantity = signal(1);
  createdId = signal<number | null>(null);

  submit() {
    const payload = {
      buyerId: this.buyerId(),
      buyerUsername: this.buyerUsername(),
      currency: this.currency(),
      paymentItems: [{
        movieName: this.movieName(),
        movieId: this.movieId(),
        price: this.price(),
        quantity: this.quantity()
      }]
    };
    this.api.createPayment(payload).subscribe(id => this.createdId.set(id));
  }
}


