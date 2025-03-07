import { Component, OnInit } from "@angular/core";
import { IMovieForStreaming, MovieStreamingService } from "../services/movie-streaming.service";

@Component({
    selector: 'private-session-list',
    imports: [],
    templateUrl: './private-session-list.component.html',
})
export class PrivateSessionsList implements OnInit {
    public movies: Array<IMovieForStreaming> = new Array();

    constructor(private moviesServices: MovieStreamingService) {

    }

    public imgForMovie(movieId: string) {
        return `http://localhost:5224/Movie/imageformovie/${movieId}`
    }

    ngOnInit(): void {
        this.moviesServices.getUpcomingMovies().subscribe((movies) => this.movies = movies);
    }
}