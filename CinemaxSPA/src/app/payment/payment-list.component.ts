import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { PaymentService } from '../services/payment.service';
import { PaymentVM } from '../types/Payment';
import { AppStateService } from '../shared/app-state/app-state.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule,
    MatTableModule,
    MatChipsModule,
    MatIconModule
  ],
  template: `
  <mat-card class="container">
    <h2 class="title">
      <mat-icon>payment</mat-icon>
      My Payments
    </h2>
    
    <div class="user-info" *ngIf="currentUser()">
      <div class="user-details">
        <p class="current-user">
          <mat-icon>person</mat-icon>
          Viewing payments for: <strong>{{ currentUser() }}</strong>
        </p>
        <p class="payment-count" *ngIf="payments().length > 0">
          <mat-icon>receipt</mat-icon>
          Total payments: {{ payments().length }}
        </p>
      </div>
      <button mat-raised-button color="primary" (click)="loadForCurrentUser()" [disabled]="isLoading()">
        <mat-icon>refresh</mat-icon>
        Refresh
      </button>
    </div>

    <form (ngSubmit)="load()" class="form-row" *ngIf="!currentUser()">
      <mat-form-field appearance="outline" class="field">
        <mat-label>Username</mat-label>
        <input matInput [ngModel]="username()" (ngModelChange)="username.set($event)" name="username" required />
      </mat-form-field>
      <button mat-raised-button color="primary" type="submit" [disabled]="isLoading()">
        {{ isLoading() ? 'Loading...' : 'Load' }}
      </button>
    </form>

    <div class="loading" *ngIf="isLoading()">
      <mat-icon>hourglass_empty</mat-icon>
      <p>Loading payments...</p>
    </div>

    <div class="empty" *ngIf="!isLoading() && payments().length === 0">
      <mat-icon>inbox</mat-icon>
      <p>No payments found for this user.</p>
      <p *ngIf="currentUser()" class="hint">Complete a PayPal payment to see it appear here.</p>
    </div>

    <div *ngIf="!isLoading() && payments().length > 0" class="payments-container">
      <div *ngFor="let payment of payments()" class="payment-card mat-elevation-z2">
        <div class="payment-header">
          <div class="payment-info">
            <h3 class="payment-id">
              <mat-icon>receipt_long</mat-icon>
              Payment #{{ payment.id }}
            </h3>
            <div class="payment-meta">
              <span class="amount">
                <mat-icon>attach_money</mat-icon>
                {{ payment.amount | number:'1.2-2' }} {{ payment.currency }}
              </span>
              <span class="buyer">
                <mat-icon>person</mat-icon>
                {{ payment.buyerUsername }}
              </span>
            </div>
          </div>
          <div class="payment-status">
            <mat-chip color="primary" selected>
              <mat-icon>check_circle</mat-icon>
              Completed
            </mat-chip>
          </div>
        </div>

        <div class="payment-details">
          <div class="items-section">
            <h4>
              <mat-icon>movie</mat-icon>
              Payment Items
            </h4>
            <div class="items-list">
              <div *ngFor="let item of payment.paymentItems" class="item-card">
                <div class="item-info">
                  <h5 class="movie-name">{{ item.movieName }}</h5>
                  <p class="movie-id">ID: {{ item.movieId }}</p>
                </div>
                <div class="item-details">
                  <span class="quantity">
                    <mat-icon>shopping_cart</mat-icon>
                    Qty: {{ item.quantity }}
                  </span>
                  <span class="price">
                    <mat-icon>attach_money</mat-icon>
                    {{ item.price | number:'1.2-2' }} {{ payment.currency }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="payment-summary">
            <div class="summary-row">
              <span class="label">Total Amount:</span>
              <span class="value total">{{ payment.totalPrice | number:'1.2-2' }} {{ payment.currency }}</span>
            </div>
            <div class="summary-row">
              <span class="label">Payment Method:</span>
              <span class="value">
                <mat-icon>paypal</mat-icon>
                PayPal
              </span>
            </div>
            <div class="summary-row">
              <span class="label">Payment Date:</span>
              <span class="value">
                <mat-icon>schedule</mat-icon>
                {{ payment.paymentDate | date:'MMM dd, yyyy HH:mm' }}
              </span>
            </div>
            <div class="summary-row">
              <span class="label">Created:</span>
              <span class="value">
                <mat-icon>access_time</mat-icon>
                {{ payment.createdDate | date:'MMM dd, yyyy HH:mm' }}
              </span>
            </div>
            <div class="summary-row">
              <span class="label">Status:</span>
              <span class="value status">
                <mat-icon>check_circle</mat-icon>
                Successfully Completed
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </mat-card>
  `,
  styles: [`
    .container { 
      padding: 1.5rem; 
      display: block; 
      max-width: 1200px;
      margin: 0 auto;
    }
    
    .title { 
      margin: 0 0 1.5rem 0; 
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: #1976d2;
      font-size: 1.8rem;
    }
    
    .user-info { 
      display: flex; 
      align-items: center; 
      justify-content: space-between;
      gap: 1rem; 
      margin-bottom: 1.5rem; 
      padding: 1.5rem; 
      background: linear-gradient(135deg, #f5f5f5 0%, #e8e8e8 100%);
      border-radius: 8px;
      border-left: 4px solid #1976d2;
    }
    
    .user-details {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    
    .current-user, .payment-count { 
      margin: 0; 
      color: #666;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    
    .payment-count {
      font-size: 0.9rem;
      font-weight: 500;
    }
    
    .form-row { 
      display: flex; 
      align-items: flex-end; 
      gap: .75rem; 
      margin-bottom: 1.5rem; 
    }
    
    .field { 
      flex: 1 1 280px; 
    }
    
    .loading, .empty { 
      text-align: center; 
      padding: 3rem 2rem; 
      color: #666;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }
    
    .loading mat-icon, .empty mat-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      color: #1976d2;
    }
    
    .hint { 
      font-style: italic; 
      font-size: 0.9rem; 
      margin-top: 0.5rem; 
      color: #888;
    }
    
    .payments-container {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }
    
    .payment-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      border: 1px solid #e0e0e0;
      transition: box-shadow 0.3s ease;
    }
    
    .payment-card:hover {
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }
    
    .payment-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1.5rem;
      padding-bottom: 1rem;
      border-bottom: 2px solid #f0f0f0;
    }
    
    .payment-info {
      flex: 1;
    }
    
    .payment-id {
      margin: 0 0 0.5rem 0;
      color: #333;
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 1.2rem;
    }
    
    .payment-meta {
      display: flex;
      gap: 1.5rem;
      flex-wrap: wrap;
    }
    
    .payment-meta span {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-weight: 500;
      color: #555;
    }
    
    .amount {
      color: #2e7d32 !important;
      font-size: 1.1rem;
    }
    
    .payment-status {
      display: flex;
      align-items: center;
    }
    
    .payment-details {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
    }
    
    .items-section h4, .payment-summary h4 {
      margin: 0 0 1rem 0;
      color: #333;
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 1rem;
    }
    
    .items-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    
    .item-card {
      background: #f8f9fa;
      border-radius: 8px;
      padding: 1rem;
      border-left: 4px solid #1976d2;
    }
    
    .item-info {
      margin-bottom: 0.75rem;
    }
    
    .movie-name {
      margin: 0 0 0.25rem 0;
      font-weight: 600;
      color: #333;
    }
    
    .movie-id {
      margin: 0;
      font-size: 0.85rem;
      color: #666;
      font-family: monospace;
    }
    
    .item-details {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    
    .item-details span {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-size: 0.9rem;
    }
    
    .price {
      font-weight: 600;
      color: #2e7d32;
    }
    
    .payment-summary {
      background: #f8f9fa;
      border-radius: 8px;
      padding: 1.5rem;
    }
    
    .summary-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.75rem;
      padding: 0.5rem 0;
    }
    
    .summary-row:last-child {
      margin-bottom: 0;
      padding-bottom: 0;
    }
    
    .label {
      font-weight: 500;
      color: #555;
    }
    
    .value {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-weight: 600;
    }
    
    .value.total {
      color: #2e7d32;
      font-size: 1.1rem;
    }
    
    .value.status {
      color: #2e7d32;
    }
    
    @media (max-width: 768px) {
      .payment-details {
        grid-template-columns: 1fr;
        gap: 1.5rem;
      }
      
      .payment-header {
        flex-direction: column;
        gap: 1rem;
      }
      
      .payment-meta {
        flex-direction: column;
        gap: 0.5rem;
      }
    }
  `]
})
export class PaymentListComponent implements OnInit {
  private readonly paymentService = inject(PaymentService);
  private readonly appStateService = inject(AppStateService);

  username = signal<string>('');
  payments = signal<PaymentVM[]>([]);
  isLoading = signal<boolean>(false);
  currentUser = signal<string>('');

  ngOnInit() {
    // Get current user from app state
    this.appStateService.getAppState().subscribe(appState => {
      if (appState.username) {
        this.currentUser.set(appState.username);
        this.username.set(appState.username);
        this.load(); // Automatically load payments for current user
      }
    });
  }

  load() {
    const u = this.username().trim();
    if (!u) return;
    
    this.isLoading.set(true);
    this.paymentService.getPayments(u).subscribe({
      next: (res) => {
        this.payments.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading payments:', err);
        this.isLoading.set(false);
      }
    });
  }

  loadForCurrentUser() {
    if (this.currentUser()) {
      this.username.set(this.currentUser());
      this.load();
    }
  }
}