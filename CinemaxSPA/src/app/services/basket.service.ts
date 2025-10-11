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
  username: string;
  items: ShoppingCartItem[];
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
          const newCart: ShoppingCart = { username, items: [] };
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
        // Prevent duplicates
        if (!cart.items.some(item => item.MovieId === movie.id)) {
          cart.items.push({
            MovieId: movie.id,
            Title: movie.title,
            ImageUrl: movie.imageUrl,
            Rating: movie.rating,
            Price: movie.price ?? 12.99
          });
          console.log(cart.items[0].Title);
          console.log("___________")
        }
        console.log(cart.items.length);
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }

  removeFromCart(username: string, movieId: string): Observable<ShoppingCart> {
    return this.getCart(username).pipe(
      map(cart => {
        console.log(movieId)
        cart.items = cart.items.filter(item => item.MovieId !== movieId);
        return cart;
      }),
      switchMap(cart =>
        this.http.put<ShoppingCart>(`${this.apiUrl}`, cart)
      )
    );
  }
}
