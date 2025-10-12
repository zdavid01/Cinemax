import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable, map, switchMap, catchError, throwError} from 'rxjs';
import { Movie } from '../types/Movie';

interface ShoppingCartItem {
  MovieId: string;
  Title: string;
  ImageUrl: string;
  Rating: string;
  Price: number;
}

interface ShoppingCart {
  Username: string;
  Items: ShoppingCartItem[];
}

@Injectable({ providedIn: 'root' })
export class BasketService {
  private apiUrl = 'http://localhost:8001/api/v1/Basket';
  constructor(private http: HttpClient) {}

  getCart(username: string): Observable<ShoppingCart> {
    return this.http.get<ShoppingCart>(`${this.apiUrl}/${username}`).pipe(
      catchError(err => {
        if (err.status === 404) {
          // Basket not found — create a new one
          const newCart: ShoppingCart = { Username: username, Items: [] };
          return this.http.put<ShoppingCart>(`${this.apiUrl}`, newCart);
        } else {
          return throwError(() => err);
        }
      })
    );
  }

  addToCart(username: string, movie: Movie): Observable<ShoppingCart> {
    return this.getCart(username).pipe(
      map(cart => {
        // Convert movie.id to string and ensure all required fields are present
        const movieId = String(movie.id);
        
        // Prevent duplicates
        if (!cart.Items.some(item => item.MovieId === movieId)) {
          cart.Items.push({
            MovieId: movieId,
            Title: movie.title || 'Unknown Title',
            ImageUrl: movie.imageUrl || '',
            Rating: String(movie.rating || '0'),
            Price: movie.price ?? 12.99
          });
          console.log(cart.Items[0].Title);
          console.log("___________")
        }
        console.log(cart.Items.length);
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }

  removeFromCart(username: string, movieId: string | number): Observable<ShoppingCart> {
    return this.getCart(username).pipe(
      map(cart => {
        // Convert movieId to string for comparison
        const movieIdStr = String(movieId);
        console.log(movieIdStr)
        cart.Items = cart.Items.filter(item => item.MovieId !== movieIdStr);
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }
}
