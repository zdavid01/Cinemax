import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../services/payment.service';
import { PaymentVM } from '../types/Payment';

@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container">
    <h2>Payments</h2>
    <form (ngSubmit)="load()" class="mb-3">
      <label>Username:
        <input [ngModel]="username()" (ngModelChange)="username.set($event)" name="username" required />
      </label>
      <button type="submit">Load</button>
    </form>
    <div *ngIf="payments().length === 0">No payments.</div>
    <table *ngIf="payments().length > 0">
      <thead>
        <tr>
          <th>Id</th>
          <th>Buyer</th>
          <th>Amount</th>
          <th>Currency</th>
          <th>Items</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let p of payments()">
          <td>{{ p.id }}</td>
          <td>{{ p.buyerUsername }}</td>
          <td>{{ p.amount | number:'1.2-2' }}</td>
          <td>{{ p.currency }}</td>
          <td>
            <ul>
              <li *ngFor="let i of p.paymentItems">{{ i.movieName }} ({{ i.quantity }}) - {{ i.price | number:'1.2-2' }}</li>
            </ul>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
  `,
  styles: [`
    .container { padding: 1rem; }
    table { width: 100%; border-collapse: collapse; }
    th, td { border: 1px solid #ddd; padding: .5rem; }
    th { background: #f6f6f6; }
  `]
})
export class PaymentListComponent {
  private readonly paymentService = inject(PaymentService);

  username = signal<string>('testuser3');
  payments = signal<PaymentVM[]>([]);

  load() {
    const u = this.username().trim();
    if (!u) return;
    this.paymentService.getPayments(u).subscribe(res => this.payments.set(res));
  }

  ngOnInit() {
    this.load();
  }
}


