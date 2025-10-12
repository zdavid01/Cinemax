import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MoviesService } from '../../services/movies.service';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movie } from '../../types/Movie';
import { BasketService } from '../../services/basket.service';

@Component({
  selector: 'app-movie-list',
  imports: [],
  templateUrl: './movie-list.component.html',
  styleUrl: './movie-list.component.css'
})
export class MovieListComponent implements OnInit{
  private movies: Movie[] = [];
  cartMovieIds: string[] = [];
  username: string = '';

  constructor(private moviesService: MoviesService, private basketService: BasketService) {}
  
  ngOnInit() {
      this.username = localStorage.getItem('username') || '';
      this.moviesService.getMovies().subscribe(movies => this.movies = movies);
      this.loadCart();
    }

  loadCart() {
    this.basketService.getCart(this.username).subscribe(cart => {
      this.cartMovieIds = cart.Items.map(item => item.MovieId);
    });
  }

  isInCart(movieId: string | number): boolean {
    return this.cartMovieIds.includes(String(movieId));
  }

  toggleCart(movie: Movie) {
    if (this.isInCart(movie.id)) {
      this.basketService.removeFromCart(this.username, movie.id).subscribe(() => this.loadCart());
    } else {
      this.basketService.addToCart(this.username, movie).subscribe(() => this.loadCart());
    }
  }

}
