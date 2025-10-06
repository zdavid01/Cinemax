import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class CatalogComponent implements OnInit {
  movies: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any[]>('http://localhost:8000/api/v1/MovieCatalog')
      .subscribe({
        next: data => {
          console.log('Fetched movies:', data);
          this.movies = data;
        },
        error: err => console.error('Error fetching movies', err)
      });
  }
}
