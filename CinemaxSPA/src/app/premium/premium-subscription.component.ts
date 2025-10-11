import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { PayPalService } from '../services/paypal.service';
import { AppStateService } from '../shared/app-state/app-state.service';
import { UserService } from '../services/user.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-premium-subscription',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  template: `
    <div class="premium-container">
      <mat-card class="premium-card">
        <mat-card-header>
          <mat-card-title class="title">
            <mat-icon class="crown-icon">workspace_premium</mat-icon>
            Cinemax Premium
          </mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <!-- Current Status -->
          <div class="status-section" [class.is-premium]="isPremium()">
            <mat-icon>{{ isPremium() ? 'verified' : 'info' }}</mat-icon>
            <div class="status-text">
              <h3>{{ isPremium() ? 'You are a Premium Member' : 'Free Account' }}</h3>
              <p>{{ isPremium() ? 'Enjoy all premium benefits!' : 'Upgrade to unlock premium features' }}</p>
            </div>
          </div>

          <!-- Premium Benefits -->
          <div class="benefits-section" *ngIf="!isPremium()">
            <h3>Premium Benefits</h3>
            <ul class="benefits-list">
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Unlimited movie streaming</span>
              </li>
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Early access to new releases</span>
              </li>
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Ad-free experience</span>
              </li>
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Exclusive premium content</span>
              </li>
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Priority customer support</span>
              </li>
              <li>
                <mat-icon>check_circle</mat-icon>
                <span>Download movies for offline viewing</span>
              </li>
            </ul>
          </div>

          <!-- Pricing -->
          <div class="pricing-section" *ngIf="!isPremium()">
            <div class="price-card">
              <div class="price-header">
                <h2>Premium Membership</h2>
                <div class="price">
                  <span class="currency">$</span>
                  <span class="amount">{{ premiumPrice }}</span>
                  <span class="period">/lifetime</span>
                </div>
              </div>
              <p class="price-description">One-time payment for lifetime access to all premium features</p>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="actions" *ngIf="!isPremium()">
            <button 
              mat-raised-button 
              color="primary" 
              class="subscribe-button"
              (click)="subscribeToPremium()"
              [disabled]="isProcessing() || !isAuthenticated()">
              <mat-spinner *ngIf="isProcessing()" diameter="20"></mat-spinner>
              <mat-icon *ngIf="!isProcessing()">payment</mat-icon>
              {{ isProcessing() ? 'Processing...' : 'Subscribe with PayPal' }}
            </button>
            <p class="auth-warning" *ngIf="!isAuthenticated()">
              <mat-icon>warning</mat-icon>
              Please login to subscribe to premium
            </p>
          </div>

          <!-- Already Premium -->
          <div class="premium-active" *ngIf="isPremium()">
            <mat-icon class="premium-icon">stars</mat-icon>
            <p>Thank you for being a premium member!</p>
            <button mat-raised-button (click)="goToHome()">
              <mat-icon>home</mat-icon>
              Go to Home
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .premium-container {
      max-width: 800px;
      margin: 40px auto;
      padding: 20px;
    }

    .premium-card {
      padding: 20px;
    }

    .title {
      display: flex;
      align-items: center;
      gap: 10px;
      font-size: 28px;
      color: #ffd700;
    }

    .crown-icon {
      font-size: 40px;
      width: 40px;
      height: 40px;
      color: #ffd700;
    }

    .status-section {
      display: flex;
      align-items: center;
      gap: 15px;
      padding: 20px;
      margin: 20px 0;
      border-radius: 8px;
      background: #f5f5f5;
      border-left: 4px solid #666;
    }

    .status-section.is-premium {
      background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
      border-left: 4px solid #ffd700;
    }

    .status-section mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
    }

    .status-text h3 {
      margin: 0 0 5px 0;
      font-size: 24px;
    }

    .status-text p {
      margin: 0;
      color: #666;
    }

    .benefits-section {
      margin: 30px 0;
    }

    .benefits-section h3 {
      margin-bottom: 20px;
      color: #333;
    }

    .benefits-list {
      list-style: none;
      padding: 0;
    }

    .benefits-list li {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      margin-bottom: 8px;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .benefits-list mat-icon {
      color: #4caf50;
      font-size: 24px;
      width: 24px;
      height: 24px;
    }

    .pricing-section {
      margin: 30px 0;
    }

    .price-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 30px;
      border-radius: 12px;
      text-align: center;
    }

    .price-header h2 {
      margin: 0 0 20px 0;
      font-size: 28px;
    }

    .price {
      display: flex;
      align-items: baseline;
      justify-content: center;
      gap: 5px;
      margin-bottom: 15px;
    }

    .currency {
      font-size: 32px;
    }

    .amount {
      font-size: 64px;
      font-weight: bold;
    }

    .period {
      font-size: 20px;
      opacity: 0.9;
    }

    .price-description {
      margin: 0;
      opacity: 0.9;
      font-size: 16px;
    }

    .actions {
      margin-top: 30px;
    }

    .subscribe-button {
      width: 100%;
      height: 56px;
      font-size: 18px;
      font-weight: bold;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
      color: white !important;
    }

    .subscribe-button mat-icon {
      margin-right: 8px;
    }

    .auth-warning {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-top: 15px;
      padding: 12px;
      background: #fff3cd;
      border-radius: 6px;
      color: #856404;
    }

    .premium-active {
      text-align: center;
      padding: 40px;
    }

    .premium-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #ffd700;
      margin-bottom: 20px;
    }

    .premium-active p {
      font-size: 18px;
      margin-bottom: 30px;
      color: #666;
    }

    .premium-active button {
      width: auto;
      padding: 12px 40px;
    }
  `]
})
export class PremiumSubscriptionComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly appStateService = inject(AppStateService);
  private readonly userService = inject(UserService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isPremium = signal<boolean>(false);
  isProcessing = signal<boolean>(false);
  isAuthenticated = signal<boolean>(false);
  username = signal<string>('');
  
  premiumPrice = 9.99; // Lifetime premium subscription price

  ngOnInit() {
    // Get current user state
    this.appStateService.getAppState().subscribe(state => {
      this.isAuthenticated.set(!!state.username);
      this.username.set(state.username || '');
      this.isPremium.set(state.isPremium || false);
    });

    // Check if returning from PayPal
    this.route.queryParams.subscribe(params => {
      if (params['payment'] === 'success' && params['username']) {
        this.handlePaymentSuccess(params['username']);
      } else if (params['payment'] === 'failed') {
        this.snackBar.open('Payment was not approved. Please try again.', 'Close', { duration: 5000 });
      } else if (params['payment'] === 'cancelled') {
        this.snackBar.open('Payment was cancelled.', 'Close', { duration: 3000 });
      } else if (params['payment'] === 'error') {
        this.snackBar.open('Payment error: ' + (params['message'] || 'Unknown error'), 'Close', { duration: 5000 });
      }
    });
  }

  subscribeToPremium() {
    if (!this.isAuthenticated()) {
      this.snackBar.open('Please login to subscribe', 'Close', { duration: 3000 });
      return;
    }

    this.isProcessing.set(true);

    // Create PayPal payment for premium subscription using dedicated endpoint
    this.http.post<any>('http://localhost:8004/api/paypal/create-premium-payment', {
      amount: this.premiumPrice,
      currency: 'USD'
    }).subscribe({
      next: (response) => {
        this.isProcessing.set(false);
        
        // Find approval URL and redirect to PayPal
        const approvalLink = response.links.find((link: any) => link.rel === 'approval_url');
        if (approvalLink) {
          // Redirect to PayPal (full page redirect)
          window.location.href = approvalLink.href;
        } else {
          this.snackBar.open('Failed to get PayPal approval URL', 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.isProcessing.set(false);
        console.error('Error creating payment:', error);
        this.snackBar.open('Failed to initiate payment. Please try again.', 'Close', { duration: 5000 });
      }
    });
  }

  private handlePaymentSuccess(username: string) {
    // Payment was successful, upgrade user to premium
    this.isProcessing.set(true);
    
    this.userService.upgradeToPremium(username).subscribe({
      next: (response) => {
        this.isProcessing.set(false);
        this.isPremium.set(true);
        
        // Update app state
        this.appStateService.setIsPremium(true);
        
        this.snackBar.open('🎉 Welcome to Cinemax Premium! Your account has been upgraded.', 'Close', { duration: 7000 });
        
        // Clear the query params
        this.router.navigate(['/premium'], { replaceUrl: true });
      },
      error: (error) => {
        this.isProcessing.set(false);
        console.error('Error upgrading to premium:', error);
        this.snackBar.open('Payment succeeded but failed to activate premium. Please contact support.', 'Close', { duration: 7000 });
      }
    });
  }

  goToHome() {
    this.router.navigate(['/catalog']);
  }
}

