import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-payment-success',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="container">
      <mat-card class="success-card" *ngIf="status === 'success'">
        <div class="icon-container success">
          <mat-icon>check_circle</mat-icon>
        </div>
        <h2>Payment Successful!</h2>
        <p>Your payment has been processed successfully.</p>
        <p class="payment-id">Payment ID: <strong>{{ paymentId }}</strong></p>
        <p class="redirect-message">
          <mat-spinner diameter="20"></mat-spinner>
          Redirecting to payments page...
        </p>
      </mat-card>

      <mat-card class="error-card" *ngIf="status === 'failed'">
        <div class="icon-container error">
          <mat-icon>error</mat-icon>
        </div>
        <h2>Payment Failed</h2>
        <p>Your payment was not approved. Please try again.</p>
        <button mat-raised-button color="primary" (click)="goToBasket()">
          Return to Basket
        </button>
      </mat-card>

      <mat-card class="error-card" *ngIf="status === 'error'">
        <div class="icon-container error">
          <mat-icon>warning</mat-icon>
        </div>
        <h2>Payment Error</h2>
        <p>An error occurred while processing your payment.</p>
        <p class="error-message">{{ errorMessage }}</p>
        <button mat-raised-button color="primary" (click)="goToBasket()">
          Return to Basket
        </button>
      </mat-card>
    </div>
  `,
  styles: [`
    .container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 80vh;
      padding: 20px;
    }
    
    .success-card, .error-card {
      max-width: 500px;
      text-align: center;
      padding: 40px;
    }
    
    .icon-container {
      margin-bottom: 20px;
    }
    
    .icon-container mat-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
    }
    
    .icon-container.success mat-icon {
      color: #4caf50;
    }
    
    .icon-container.error mat-icon {
      color: #f44336;
    }
    
    h2 {
      margin: 10px 0 20px 0;
      color: #333;
    }
    
    p {
      color: #666;
      margin: 10px 0;
    }
    
    .payment-id {
      font-size: 14px;
      color: #888;
    }
    
    .redirect-message {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 10px;
      margin-top: 30px;
      color: #1976d2;
      font-weight: 500;
    }
    
    .error-message {
      background-color: #ffeaea;
      padding: 10px;
      border-radius: 4px;
      font-size: 14px;
      word-break: break-word;
    }
    
    button {
      margin-top: 20px;
      width: 100%;
      padding: 12px;
    }
  `]
})
export class PaymentSuccessComponent implements OnInit {
  status: string = '';
  paymentId: string = '';
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    // Get query parameters
    this.route.queryParams.subscribe(params => {
      this.status = params['status'] || '';
      this.paymentId = params['paymentId'] || '';
      this.errorMessage = params['message'] || '';

      // If payment was successful, redirect to payments page after 2 seconds
      if (this.status === 'success') {
        setTimeout(() => {
          // Navigate and force component reload by using skipLocationChange and then navigating again
          this.router.navigate(['/payments'], { 
            queryParams: { refresh: new Date().getTime() }
          });
        }, 2000);
      }
    });
  }

  goToBasket() {
    this.router.navigate(['/basket']);
  }
}

