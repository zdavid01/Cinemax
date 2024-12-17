import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Movie } from '../types/Movie';
import { Observable, of } from 'rxjs';
import dbData from "../db.json"


@Injectable({
  providedIn: 'root'
})
export class MoviesService {
  private baseApiUrl = 'http://localhost:3000/movies';

  constructor(private http: HttpClient) { }

  getMovies(): Observable<Movie[]> {
    
    return of(dbData.movies);
    // return this.http.get<Movies[]>(`${this.baseApiUrl}`);
  }

  updateMovie(movie: Movie): Observable<Movie> {
    return of();
    // const url = `${this.baseApiUrl}/${movie.id}`;
    // return this.http.put<Movies>(url, movie, httpOptions);
  }

  addMovie(movie: Movie): Observable<Movie> {
    return of();
    // const url = `${this.baseApiUrl}`;
    // return this.http.post<Movies>(url, movie, httpOptions);
  }
}
