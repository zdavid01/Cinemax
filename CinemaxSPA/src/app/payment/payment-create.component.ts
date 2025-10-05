import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { PaymentService } from '../services/payment.service';

@Component({
  selector: 'app-payment-create',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
  <mat-card class="container">
    <h2 class="title">Create Payment</h2>
    <form (ngSubmit)="submit()" class="form-grid">
      <mat-form-field appearance="outline">
        <mat-label>Buyer Id</mat-label>
        <input matInput [ngModel]="buyerId()" (ngModelChange)="buyerId.set($event)" name="buyerId" required />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Buyer Username</mat-label>
        <input matInput [ngModel]="buyerUsername()" (ngModelChange)="buyerUsername.set($event)" name="buyerUsername" required />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Currency</mat-label>
        <input matInput [ngModel]="currency()" (ngModelChange)="currency.set($event)" name="currency" required />
      </mat-form-field>

      <div class="section">Item</div>

      <mat-form-field appearance="outline">
        <mat-label>Movie Name</mat-label>
        <input matInput [ngModel]="movieName()" (ngModelChange)="movieName.set($event)" name="movieName" required />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Movie Id</mat-label>
        <input matInput [ngModel]="movieId()" (ngModelChange)="movieId.set($event)" name="movieId" required />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Price</mat-label>
        <input matInput type="number" step="0.01" [ngModel]="price()" (ngModelChange)="price.set($event)" name="price" required />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Quantity</mat-label>
        <input matInput type="number" [ngModel]="quantity()" (ngModelChange)="quantity.set($event)" name="quantity" required />
      </mat-form-field>

      <div class="actions">
        <button mat-raised-button color="primary" type="submit">Create</button>
      </div>
    </form>
    <p class="success" *ngIf="createdId()">Created payment with id {{ createdId() }}</p>
  </mat-card>
  `,
  styles: [`
    .container { padding: 1rem; display: block; }
    .title { margin: 0 0 1rem 0; }
    .form-grid { display: grid; grid-template-columns: repeat(2, minmax(220px, 1fr)); gap: .75rem; max-width: 720px; }
    .section { grid-column: 1 / -1; font-weight: 600; margin-top: .5rem; }
    .actions { grid-column: 1 / -1; }
    .success { margin-top: .75rem; }
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


