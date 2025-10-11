import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BasketService } from '../services/basket.service'
import { PayPalService } from '../services/paypal.service';
import { Movie } from '../types/Movie';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AppStateService } from '../shared/app-state/app-state.service';
import { take } from 'rxjs';

interface CartItem extends Movie {
  price?: number;
}

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, MatSnackBarModule],
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})

export class BasketComponent implements OnInit {
  cartItems: CartItem[] = [];
  username: string = '';
  isProcessing: boolean = false;

  constructor(
    private basketService: BasketService,
    private paypalService: PayPalService,
    private appStateService: AppStateService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    // Get username from AppStateService
    this.appStateService.getAppState().pipe(take(1)).subscribe(appState => {
      this.username = appState.username || '';
      this.loadCart();
    });
  }

  loadCart() {
    if (!this.username) {
      console.warn('No username found. User needs to login first.');
      this.cartItems = [];
      return;
    }
    
    this.basketService.getCart(this.username).subscribe({
      next: (cart) => {
        this.cartItems = cart.items.map((item: any) => ({
          id: item.movieId,
          title: item.title,
          length: 0,
          genre: '',
          director: '',
          actors: '',
          description: '',
          imageUrl: item.imageUrl,
          trailerLink: '',
          rating: item.rating,
          price: item.price || 12.99 // Default price if not provided
        }));
      },
      error: (error) => {
        console.error('Error loading basket:', error);
        if (error.status === 401) {
          console.warn('Unauthorized. Please login first.');
        } else if (error.status === 404) {
          console.log('Basket is empty or not found.');
          this.cartItems = [];
        } else {
          this.cartItems = [];
        }
      }
    });
  }

  removeFromCart(movieId: string) { 
    this.basketService.removeFromCart(this.username, movieId).subscribe(() => this.loadCart());
  }

  getTotalMovies(): number {
    return this.cartItems.length;
  }

  getTotalPrice(): number {
    return this.cartItems.reduce((sum, item) => sum + (item.price || 12.99), 0);
  }

  getAverageRating(): string {
    if (!this.cartItems.length) return '0.0';
    const avg = this.cartItems.reduce((sum, m) => sum + (parseFloat(m.rating) || 0), 0) / this.cartItems.length;
    return avg.toFixed(1);
  }

  goToCheckout() {
    if (this.cartItems.length === 0) {
      this.snackBar.open('Your basket is empty!', 'Close', { duration: 3000 });
      return;
    }

    this.isProcessing = true;
    const totalAmount = this.getTotalPrice();

    // Create PayPal payment
    this.paypalService.createPayment({
      amount: totalAmount,
      currency: 'USD'
    }).subscribe({
      next: (response) => {
        this.isProcessing = false;
        // Store payment details for later
        localStorage.setItem('pendingPaymentId', response.id);
        localStorage.setItem('basketTotal', totalAmount.toString());
        
        // Find approval URL
        const approvalLink = response.links.find(link => link.rel === 'approval_url');
        if (approvalLink) {
          // Redirect to PayPal
          window.location.href = approvalLink.href;
        } else {
          this.snackBar.open('Failed to get PayPal approval URL', 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.isProcessing = false;
        console.error('Error creating payment:', error);
        this.snackBar.open('Failed to initiate payment. Please try again.', 'Close', { duration: 5000 });
      }
    });
  }

  handleImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    target.src = 'assets/img6.png'; // fallback image
  }
}