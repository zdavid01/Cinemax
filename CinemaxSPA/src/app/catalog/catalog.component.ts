import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';
import {AppStateService} from '../shared/app-state/app-state.service';
import { BasketService } from '../services/basket.service';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatSnackBarModule]
})
export class CatalogComponent implements OnInit {
  movies: any[] = [];
  username: string = '';

  constructor(
    private http: HttpClient,
    private appStateService: AppStateService,
    private snackBar: MatSnackBar,
    private basketService: BasketService) {}

  ngOnInit(): void {
    this.http.get<any[]>('http://localhost:8000/api/v1/MovieCatalog')
      .subscribe({
        next: data => {
          console.log('Fetched movies:', data);
          this.movies = data;
        },
        error: err => console.error('Error fetching movies', err)
      });

    this.appStateService.getAppState().subscribe(appState => {
      this.username = appState.username || '';
      console.log('Logged in username:', this.username);
    });
  }

  addToBasket(movie: any): void {
    if (!this.username) {
      this.snackBar.open('Please login first', 'Close', { duration: 3000 });
      return;
    }

    this.basketService.addToCart(this.username, movie).subscribe({
      next: () => {
        this.snackBar.open(`${movie.title} added to basket`, 'Close', { duration: 3000 });
      },
      error: err => {
        console.error('Error adding to basket:', err);
        this.snackBar.open('Failed to add to basket', 'Close', { duration: 3000 });
      }
    });
  }
}
