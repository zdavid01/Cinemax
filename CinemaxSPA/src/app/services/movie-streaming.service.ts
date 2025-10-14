import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";

export interface IMovieForStreaming {
    id: string;    
    description: string;
    title: string,
    imageUrl: string,
    releaseDate?: string,
    expiresInDays?: number
}

@Injectable({
    providedIn: 'root'
})
export class MovieStreamingService {
    // Use Docker port 8005 for PrivateSession API
    private readonly API_URL = 'http://localhost:8005/Movie';

    constructor(private httpClient: HttpClient) {

    }

    getUpcomingMovies(): Observable<Array<IMovieForStreaming>> {
        // Updated endpoint to match Google Drive API
        return this.httpClient.get<Array<IMovieForStreaming>>(`${this.API_URL}/movies`);
    }

    getMovieMetadata(id: string): Observable<IMovieForStreaming> {
        return this.httpClient.get<IMovieForStreaming>(`${this.API_URL}/${id}`)
    }

    getStreamUrl(id: string): string {
        return `${this.API_URL}/stream/${id}`;
    }

    getImageUrl(id: string): string {
        return `${this.API_URL}/imageForMovie/${id}`;
    }
}