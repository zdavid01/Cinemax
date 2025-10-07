import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasketService } from '../services/basket.service'
import { Movie } from '../types/Movie';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

const username = localStorage.getItem('username') || "";

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule],
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})

export class BasketComponent implements OnInit {
  cartItems: Movie[] = [];

  constructor(private basketService: BasketService) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.basketService.getCart(username).subscribe(cart => {
      this.cartItems = cart.items.map((item: any) => ({
        id: item.id,
        title: item.title,
        rating: item.rating,
        imageUrl: item.imageUrl
      }));
    });
  }

  removeFromCart(movieId: number) { 
    this.basketService.removeFromCart(username, movieId).subscribe(() => this.loadCart());
  }

  getTotalMovies(): number {
    return this.cartItems.length;
  }

  getAverageRating(): string {
    if (!this.cartItems.length) return '0.0';
    const avg = this.cartItems.reduce((sum, m) => sum + (m.rating || 0), 0) / this.cartItems.length;
    return avg.toFixed(1);
  }

  handleImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    target.src = 'assets/img6.png'; // fallback image
  }
}