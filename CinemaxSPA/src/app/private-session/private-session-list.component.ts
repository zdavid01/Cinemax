import { Component, OnInit } from "@angular/core";
import { IMovieForStreaming, MovieStreamingService } from "../services/movie-streaming.service";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";

@Component({
    selector: 'private-session-list',
    imports: [CommonModule, RouterModule],
    templateUrl: './private-session-list.component.html',
})
export class PrivateSessionsList implements OnInit {
    public movies: Array<IMovieForStreaming> = new Array();
    public loading: boolean = true;
    public error: string = '';

    constructor(private moviesServices: MovieStreamingService) {

    }

    public imgForMovie(movieId: string) {
        return this.moviesServices.getImageUrl(movieId);
    }

    ngOnInit(): void {
        this.moviesServices.getUpcomingMovies().subscribe({
            next: (movies) => {
                this.movies = movies;
                this.loading = false;
                console.log('Loaded movies from Google Drive:', movies);
            },
            error: (err) => {
                console.error('Error loading movies:', err);
                this.error = 'Failed to load movies from Google Drive. Make sure you are authenticated.';
                this.loading = false;
            }
        });
    }
}