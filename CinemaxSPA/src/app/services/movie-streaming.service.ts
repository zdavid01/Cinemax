import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";

export interface IMovieForStreaming {
    id: string;    
    description: string;
    title: string,
    imageUrl: string
}

@Injectable({
    providedIn: 'root'
})
export class MovieStreamingService {
    constructor(private httpClient: HttpClient) {

    }

    getUpcomingMovies(): Observable<Array<IMovieForStreaming>> {
        return this.httpClient.get<Array<IMovieForStreaming>>('http://localhost:5224/Movie/Movies');
    }

    getMovieMetadata(id: string): Observable<IMovieForStreaming> {
        return this.httpClient.get<IMovieForStreaming>(`http://localhost:5224/Movie/${id}`)
    }

}