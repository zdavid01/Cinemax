import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasketService } from '../services/basket.service'
import { Movie } from '../types/Movie';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AppStateService } from '../shared/app-state/app-state.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule],
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})

export class BasketComponent implements OnInit {
  cartItems: Movie[] = [];
  username: string = '';

  constructor(
    private basketService: BasketService,
    private appStateService: AppStateService
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
          rating: item.rating
        }));
      },
      error: (error) => {
        console.error('Error loading basket:', error);
        if (error.status === 401) {
          console.warn('Unauthorized. Please login first.');
        }
        this.cartItems = [];
      }
    });
  }

  removeFromCart(movieId: string) { 
    this.basketService.removeFromCart(this.username, movieId).subscribe(() => this.loadCart());
  }

  getTotalMovies(): number {
    return this.cartItems.length;
  }

  getAverageRating(): string {
    if (!this.cartItems.length) return '0.0';
    const avg = this.cartItems.reduce((sum, m) => sum + (parseFloat(m.rating) || 0), 0) / this.cartItems.length;
    return avg.toFixed(1);
  }

  handleImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    target.src = 'assets/img6.png'; // fallback image
  }
}