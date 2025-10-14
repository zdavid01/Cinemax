import { Component, OnInit } from '@angular/core';
import { ChatComponent } from "../chat/chat.component";
import { IMovieForStreaming, MovieStreamingService } from '../services/movie-streaming.service';
import { CommonModule } from '@angular/common';
import { VideoPlayerComponent } from "./video-player";
import { ActivatedRoute } from '@angular/router';
import { LocalStorageService } from '../shared/local-storage/local-storage.service';
import { LocalStorageKeys } from '../shared/local-storage/local-storage-keys';


@Component({
  selector: 'app-private-session',
  imports: [CommonModule, ChatComponent, VideoPlayerComponent],
  templateUrl: './private-session.component.html',
})
export class PrivateSessionComponent implements OnInit {

  public movieForStreaming: IMovieForStreaming | null = null;
  public error: string = "";
  public movieStreamUrl = "";
  public movieId: string = "";
  public loading: boolean = true;

  constructor(
    private movieStreamingService: MovieStreamingService, 
    private route: ActivatedRoute, 
    private localStorageService: LocalStorageService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.movieId = params["sessionId"];
      this.localStorageService.set(LocalStorageKeys.SessionId, this.movieId);
      this.loadMovie();
    });
  }

  private loadMovie(): void {
    this.movieStreamingService.getMovieMetadata(this.movieId).subscribe({
      next: (movie: IMovieForStreaming) => {
        this.movieForStreaming = movie;
        this.movieStreamUrl = this.movieStreamingService.getStreamUrl(this.movieId);
        this.loading = false;
        console.log('Movie loaded from Google Drive:', movie);
        console.log('Stream URL:', this.movieStreamUrl);
      },
      error: (err) => {
        console.error('Error loading movie:', err);
        this.error = 'Failed to load movie from Google Drive. Please make sure you are authenticated.';
        this.loading = false;
      }
    });
  }
}
