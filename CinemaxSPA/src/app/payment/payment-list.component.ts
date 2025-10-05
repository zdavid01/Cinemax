import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { PaymentService } from '../services/payment.service';
import { PaymentVM } from '../types/Payment';

@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
  <mat-card class="container">
    <h2 class="title">Payments</h2>
    <form (ngSubmit)="load()" class="form-row">
      <mat-form-field appearance="outline" class="field">
        <mat-label>Username</mat-label>
        <input matInput [ngModel]="username()" (ngModelChange)="username.set($event)" name="username" required />
      </mat-form-field>
      <button mat-raised-button color="primary" type="submit">Load</button>
    </form>

    <div class="empty" *ngIf="payments().length === 0">No payments found.</div>

    <div *ngIf="payments().length > 0" class="table-wrap mat-elevation-z2">
      <table>
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
              <ul class="items">
                <li *ngFor="let i of p.paymentItems">{{ i.movieName }} ({{ i.quantity }}) - {{ i.price | number:'1.2-2' }}</li>
              </ul>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </mat-card>
  `,
  styles: [`
    .container { padding: 1rem; display: block; }
    .title { margin: 0 0 1rem 0; }
    .form-row { display: flex; align-items: flex-end; gap: .75rem; margin-bottom: 1rem; }
    .field { flex: 1 1 280px; }
    .table-wrap { overflow: auto; border-radius: 4px; }
    table { width: 100%; border-collapse: collapse; background: #fff; }
    thead tr { background: #fafafa; }
    th, td { padding: .75rem; text-align: left; border-bottom: 1px solid #eee; vertical-align: top; }
    .items { margin: 0; padding-left: 1rem; }
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


