import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';
import {AppStateService} from '../shared/app-state/app-state.service';
import { BasketService } from '../services/basket.service';
import { FormsModule } from '@angular/forms';
import {Movie} from '../types/Movie';
import { SafeUrlPipe } from '../pipes/safe-url.pipe';
import { Role } from '../shared/app-state/role';


@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  standalone: true,
  imports: [CommonModule, MatButtonModule, FormsModule, MatCardModule, MatSnackBarModule, SafeUrlPipe]
})

export class CatalogComponent implements OnInit {
  movies: any[] = [];
  username: string = '';
  isAdmin: boolean = false;
  showAddMovieForm = false;
  isEditing = false;
  selectedMovie: any = null;
  private authToken: string = '';

  editMovieForm: any = {
    id: null,
    title: '',
    genre: '',
    director: '',
    length: 0,
    imageUrl: '',
    linkToTrailer: '',
    description: '',
    actors: '',
    rating: '',
    price: 0.0
  };

  newMovie: any = {
    Title: '',
    Genre: '',
    Director: '',
    Length: 0,
    ImageUrl: '',
    linkToTrailer: '',
    Description: '',
    Actors: '',
    Rating: '',
    Price: 0.0
  };


  constructor(
    private http: HttpClient,
    private appStateService: AppStateService,
    private snackBar: MatSnackBar,
    private basketService: BasketService
  ) {
  }

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
      this.authToken = appState.accessToken || '';
      
      // Check if user has Admin role
      if (appState.roles) {
        if (Array.isArray(appState.roles)) {
          this.isAdmin = appState.roles.includes(Role.Administrator);
        } else {
          this.isAdmin = appState.roles === Role.Administrator;
        }
      } else {
        this.isAdmin = false;
      }
      
      console.log('Logged in username:', this.username);
      console.log('Is Admin:', this.isAdmin);
      console.log('Roles:', appState.roles);
    });
  }

  addMovie() {
    if (!this.newMovie.Title) {
      this.snackBar.open('Movie title is required', 'Close', {duration: 3000});
      return;
    }

    this.newMovie.Length = Number(this.newMovie.Length);
    this.newMovie.Price = Number(this.newMovie.Price);

    console.log('Posting movie:', this.newMovie);
    this.http.post('http://localhost:8000/api/v1/MovieCatalog', this.newMovie, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.authToken}`
      }
    }).subscribe({
        next: (movie: any) => {
          this.movies.push(movie);
          this.snackBar.open('Movie added!', 'Close', {duration: 3000});
          this.newMovie = {
            Title: '',
            Genre: '',
            Director: '',
            Length: 0,
            ImageUrl: '',
            linkToTrailer: '',
            Description: '',
            Actors: '',
            Rating: '',
            Price: 0
          };
          this.showAddMovieForm = false;
        },
        error: err => {
          console.error('Error adding movie', err);
          this.snackBar.open('Failed to add movie', 'Close', {duration: 3000});
        }
      });
  }

  addToBasket(movie: any): void {
    if (!this.username) {
      this.snackBar.open('Please login first', 'Close', {duration: 3000});
      return;
    }

    this.basketService.addToCart(this.username, movie).subscribe({
      next: () => {
        this.snackBar.open(`${movie.title} added to basket`, 'Close', {duration: 3000});
        console.log("Added to basket", movie.title)
      },
      error: err => {
        console.error('Error adding to basket:', err);
        this.snackBar.open('Failed to add to basket', 'Close', {duration: 3000});
      }
    });
  }

  deleteMovie(movie: any) {
    if (!confirm(`Are you sure you want to delete "${movie.title}"?`)) return;
    console.log('Deleting movie: ', movie.title);

    this.http.delete(`http://localhost:8000/api/v1/MovieCatalog/${movie.id}`, {
      headers: {
        'Authorization': `Bearer ${this.authToken}`
      }
    }).subscribe({
        next: () => {
          this.movies = this.movies.filter(m => m.id !== movie.id);
          this.snackBar.open('Movie deleted!', 'Close', {duration: 3000});
        },
        error: err => {
          console.error('Error deleting movie', err);
          this.snackBar.open('Failed to delete movie', 'Close', {duration: 3000});
        }
      });
  }

  editMovie(movie: any) {
    this.editMovieForm = {...movie};
    this.editMovieForm.length = Number(this.editMovieForm.length);
    this.editMovieForm.price = Number(this.editMovieForm.price)
    this.isEditing = true;
    this.showAddMovieForm = false;
    console.log('Editing movie', this.editMovieForm);
  }

  updateMovie() {

    this.editMovieForm.length = Number(this.editMovieForm.length);
    this.editMovieForm.price = Number(this.editMovieForm.price);

    const movieId = this.editMovieForm.id;

    this.http.put(`http://localhost:8000/api/v1/MovieCatalog`, this.editMovieForm, {
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.authToken}`
      }
    }).subscribe({
        next: (updatedMovie: any) => {
          if (!updatedMovie) {
            const index = this.movies.findIndex(m => m.id === movieId);
            if (index > -1) this.movies[index] = { ...this.editMovieForm };
          } else {
            const index = this.movies.findIndex(m => m.id === updatedMovie.id);
            if (index > -1) this.movies[index] = updatedMovie;
          }

          this.snackBar.open('Movie updated!', 'Close', { duration: 3000 });
          this.resetEditForm();
        },
        error: err => {
          console.error('Error updating movie', err);
          this.snackBar.open('Failed to update movie', 'Close', { duration: 3000 });
        }
      });

    this.isEditing = false;
    this.showAddMovieForm = true;
    console.log(this.editMovieForm);
  }

  resetEditForm() {
    this.isEditing = false;
    this.editMovieForm = {
      id: null,
      title: '',
      genre: '',
      director: '',
      length: 0,
      imageUrl: '',
      linkToTrailer: '',
      description: '',
      actors: '',
      rating: '',
      price: 0.0
    };
  }

  openMovieDetails(movie: any) {
    this.selectedMovie = movie;
  }

  closeMovieDetails() {
    this.selectedMovie = null;
  }

  getEmbedUrl(url: string): string {
    if (!url) return '';

    const videoIdMatch = url.match(/(?:v=|youtu\.be\/)([a-zA-Z0-9_-]{11})/);
    if (videoIdMatch && videoIdMatch[1]) {
      return `https://www.youtube.com/embed/${videoIdMatch[1]}`;
    }
    return url;
  }

}
