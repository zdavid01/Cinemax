import { Component, signal } from '@angular/core';
import { PayPalService } from '../services/paypal.service';
import { PayPalPaymentRequest, PayPalExecuteRequest } from '../types/PayPal';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-paypal-payment',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule, 
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <mat-card class="payment-card">
      <mat-card-header>
        <mat-card-title>PayPal Payment</mat-card-title>
        <mat-card-subtitle>Test the complete PayPal workflow</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        
        <!-- Step 1: Create Payment -->
        <div *ngIf="currentStep() === 'create'" class="step-container">
          <h3>Step 1: Create Payment</h3>
          <form (ngSubmit)="createPayment()">
            <div class="form-grid">
              <mat-form-field appearance="fill">
                <mat-label>Amount</mat-label>
                <input matInput type="number" step="0.01" [ngModel]="amount()" (ngModelChange)="amount.set($event)" name="amount" required />
              </mat-form-field>
              <mat-form-field appearance="fill">
                <mat-label>Currency</mat-label>
                <input matInput [ngModel]="currency()" (ngModelChange)="currency.set($event)" name="currency" required />
              </mat-form-field>
            </div>
            <button mat-raised-button color="primary" type="submit" [disabled]="isLoading()">
              <mat-spinner *ngIf="isLoading()" diameter="20"></mat-spinner>
              {{ isLoading() ? 'Creating...' : 'Create Payment' }}
            </button>
          </form>
        </div>

        <!-- Step 2: PayPal Approval -->
        <div *ngIf="currentStep() === 'approve'" class="step-container">
          <h3>Step 2: PayPal Approval</h3>
          <p>Payment created successfully! Payment ID: <strong>{{ paymentId() }}</strong></p>
          <p>In a real scenario, you would be redirected to PayPal for approval.</p>
          <p>For testing purposes, you can simulate the approval process:</p>
          
          <div class="approval-buttons">
            <button mat-raised-button color="warn" (click)="simulateApproval('denied')">
              Simulate Denial
            </button>
            <button mat-raised-button color="accent" (click)="simulateApproval('approved')">
              🧪 Simulate Approval (Testing Only)
            </button>
          </div>
          
          <div class="paypal-link" *ngIf="paypalApprovalUrl()">
            <h4>🔗 Real PayPal Approval (Recommended)</h4>
            <p>For real testing with PayPal sandbox:</p>
            <button mat-raised-button color="primary" style="background-color: #0070ba; color: white; margin: 10px 0;" (click)="openPayPalApproval()">
              🏦 Approve Payment on PayPal Sandbox
            </button>
            <p class="approval-note">
              <strong>Note:</strong> Use PayPal test credentials to approve the payment. 
              After approval, PayPal will redirect back with a real payer ID.
            </p>
          </div>
        </div>

        <!-- Step 3: Execute Payment -->
        <div *ngIf="currentStep() === 'execute'" class="step-container">
          <h3>Step 3: Execute Payment</h3>
          <p>Simulating payment execution with Payment ID: <strong>{{ paymentId() }}</strong></p>
          <p>Payer ID: <strong>{{ payerId() }}</strong></p>
          <button mat-raised-button color="primary" (click)="executePayment()" [disabled]="isLoading()">
            <mat-spinner *ngIf="isLoading()" diameter="20"></mat-spinner>
            {{ isLoading() ? 'Executing...' : 'Execute Payment' }}
          </button>
        </div>

        <!-- Step 4: Success -->
        <div *ngIf="currentStep() === 'success'" class="step-container success">
          <h3>✅ Payment Successful!</h3>
          <p><strong>Payment ID:</strong> {{ paymentId() }}</p>
          <p><strong>Amount:</strong> {{ currency() }} {{ amount() }}</p>
          <p><strong>Status:</strong> {{ paymentStatus() }}</p>
          <p class="email-notice">📧 A confirmation email has been sent!</p>
          <button mat-raised-button color="primary" (click)="resetWorkflow()">
            Start New Payment
          </button>
        </div>

        <!-- Step 5: Error -->
        <div *ngIf="currentStep() === 'error'" class="step-container error">
          <h3>❌ Payment Failed</h3>
          <p>{{ errorMessage() }}</p>
          <button mat-raised-button color="primary" (click)="resetWorkflow()">
            Try Again
          </button>
        </div>

      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .payment-card {
      max-width: 600px;
      margin: 20px auto;
      padding: 20px;
    }
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 10px;
      margin-bottom: 20px;
    }
    .step-container {
      margin: 20px 0;
    }
    .step-container.success {
      background-color: #e8f5e8;
      padding: 20px;
      border-radius: 8px;
      border-left: 4px solid #4caf50;
    }
    .step-container.error {
      background-color: #ffeaea;
      padding: 20px;
      border-radius: 8px;
      border-left: 4px solid #f44336;
    }
    .approval-buttons {
      display: flex;
      gap: 10px;
      margin: 20px 0;
    }
    .paypal-link {
      margin-top: 20px;
      padding: 15px;
      background-color: #f5f5f5;
      border-radius: 8px;
    }
    .paypal-redirect {
      color: #0070ba;
      text-decoration: none;
      font-weight: bold;
    }
    .paypal-redirect:hover {
      text-decoration: underline;
    }
    .email-notice {
      background-color: #e3f2fd;
      padding: 10px;
      border-radius: 4px;
      margin: 10px 0;
    }
    mat-form-field {
      width: 100%;
    }
    button {
      width: 100%;
      padding: 10px;
      margin: 5px 0;
    }
    mat-spinner {
      margin-right: 10px;
    }
  `]
})
export class PayPalPaymentComponent {
  // Form inputs
  amount = signal(0.10);
  currency = signal('USD');
  
  // Workflow state
  currentStep = signal<'create' | 'approve' | 'execute' | 'success' | 'error'>('create');
  isLoading = signal(false);
  paymentId = signal('');
  payerId = signal('');
  paypalApprovalUrl = signal('');
  paymentStatus = signal('');
  errorMessage = signal('');

  private paypalPopup: Window | null = null;

  constructor(
    private paypalService: PayPalService, 
    private snackBar: MatSnackBar
  ) { 
    // Listen for messages from PayPal popup
    window.addEventListener('message', this.handlePayPalMessage.bind(this));
  }

  createPayment(): void {
    this.isLoading.set(true);
    const request: PayPalPaymentRequest = {
      amount: this.amount(),
      currency: this.currency()
    };

    this.paypalService.createPayment(request).subscribe({
      next: (response) => {
        this.paymentId.set(response.id);
        this.paypalApprovalUrl.set(this.findApprovalUrl(response.links));
        this.currentStep.set('approve');
        this.isLoading.set(false);
        this.snackBar.open('Payment created successfully!', 'Close', { duration: 3000 });
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to create payment');
        this.currentStep.set('error');
        this.snackBar.open('Failed to create payment', 'Close', { duration: 3000 });
      }
    });
  }

  simulateApproval(decision: 'approved' | 'denied'): void {
    if (decision === 'denied') {
      this.errorMessage.set('Payment was denied by the user');
      this.currentStep.set('error');
    } else {
      // Simulate PayPal approval - in real scenario, PayPal would redirect back with payerId
      this.payerId.set('TEST_PAYER_ID_' + Date.now());
      this.currentStep.set('execute');
      this.snackBar.open('Payment approved! Now executing...', 'Close', { duration: 3000 });
    }
  }

  executePayment(): void {
    this.isLoading.set(true);
    const request: PayPalExecuteRequest = {
      paymentId: this.paymentId(),
      payerId: this.payerId()
    };

    this.paypalService.executePayment(request).subscribe({
      next: (response) => {
        this.paymentStatus.set(response.state);
        this.currentStep.set('success');
        this.isLoading.set(false);
        this.snackBar.open('Payment executed successfully! Email sent.', 'Close', { duration: 5000 });
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to execute payment');
        this.currentStep.set('error');
        this.snackBar.open('Failed to execute payment', 'Close', { duration: 3000 });
      }
    });
  }

  resetWorkflow(): void {
    this.currentStep.set('create');
    this.paymentId.set('');
    this.payerId.set('');
    this.paypalApprovalUrl.set('');
    this.paymentStatus.set('');
    this.errorMessage.set('');
    this.isLoading.set(false);
  }

  private findApprovalUrl(links: Array<{href: string, rel: string, method: string}>): string {
    const approvalLink = links.find(link => link.rel === 'approval_url');
    return approvalLink?.href || '';
  }

  openPayPalApproval(): void {
    if (this.paypalApprovalUrl()) {
      // Open PayPal approval in a popup window
      this.paypalPopup = window.open(
        this.paypalApprovalUrl(),
        'paypal-approval',
        'width=600,height=700,scrollbars=yes,resizable=yes'
      );
      
      if (!this.paypalPopup) {
        this.snackBar.open('Popup blocked! Please allow popups for this site.', 'Close', { duration: 5000 });
        return;
      }
      
      this.snackBar.open('PayPal approval window opened. Please complete the payment process.', 'Close', { duration: 5000 });
    }
  }

  private handlePayPalMessage(event: MessageEvent): void {
    // Only handle messages from PayPal popup
    if (event.source !== this.paypalPopup) return;
    
    if (event.data.type === 'PAYPAL_PAYMENT_SUCCESS') {
      // Payment was successful, update the UI
      this.payerId.set(event.data.payerId);
      this.paymentStatus.set(event.data.state);
      this.currentStep.set('success');
      this.snackBar.open('Payment completed successfully!', 'Close', { duration: 5000 });
      
      // Close the popup
      if (this.paypalPopup) {
        this.paypalPopup.close();
        this.paypalPopup = null;
      }
    } else if (event.data.type === 'PAYPAL_PAYMENT_FAILED') {
      // Payment failed
      this.errorMessage.set('Payment was not approved by PayPal');
      this.currentStep.set('error');
      this.snackBar.open('Payment failed. Please try again.', 'Close', { duration: 5000 });
      
      // Close the popup
      if (this.paypalPopup) {
        this.paypalPopup.close();
        this.paypalPopup = null;
      }
    }
  }
}
