import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, switchMap } from 'rxjs';
import { Movie } from '../types/Movie';

interface ShoppingCartItem {
  movieId: number;
  title: string;
  imageUrl: string;
  rating: number;
}

interface ShoppingCart {
  username: string;
  items: ShoppingCartItem[];
}

@Injectable({ providedIn: 'root' })
export class BasketService {
  private apiUrl = 'http://localhost:4200/api/v1/Basket'; 
  constructor(private http: HttpClient) {}

  getCart(username: string): Observable<ShoppingCart> {
    return this.http.get<ShoppingCart>(`${this.apiUrl}/${username}`);
  }

  addToCart(username: string, movie: Movie): Observable<ShoppingCart> {
    return this.getCart(username).pipe(
      map(cart => {
        // Prevent duplicates
        if (!cart.items.some(item => item.movieId === movie.id)) {
          cart.items.push({
            movieId: movie.id,
            title: movie.title,
            imageUrl: movie.imageUrl,
            rating: movie.rating
          });
        }
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }

  removeFromCart(username: string, movieId: number): Observable<ShoppingCart> {
    return this.getCart(username).pipe(
      map(cart => {
        cart.items = cart.items.filter(item => item.movieId !== movieId);
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }
}