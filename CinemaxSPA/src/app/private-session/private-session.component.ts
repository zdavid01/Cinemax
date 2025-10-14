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
      next: async (movie: IMovieForStreaming) => {
        this.movieForStreaming = movie;
        
        // Check if we should use HLS or direct streaming
        // Try HLS first, fallback to direct stream
        const hlsUrl = `/Movie/hls/${this.movieId}/playlist.m3u8`;
        const directUrl = `/Movie/stream/${this.movieId}`;
        
        // Test if HLS endpoint exists (will return 404 if it's not an HLS folder)
        try {
          const authToken = this.getAuthToken();
          const hlsTestUrl = `http://localhost:8005${hlsUrl}?access_token=${encodeURIComponent(authToken)}`;
          
          console.log('🔍 Testing HLS endpoint:', hlsTestUrl.substring(0, 60) + '...');
          
          const response = await fetch(hlsTestUrl, {
            method: 'HEAD'
          });
          
          console.log('🔍 HEAD response status:', response.status, response.statusText);
          
          if (response.ok) {
            // HLS endpoint exists - use it
            this.movieStreamUrl = hlsUrl;
            console.log('📺 Using HLS streaming for', this.movieId);
          } else {
            // Not HLS - use direct stream
            this.movieStreamUrl = directUrl;
            console.log('🎬 Using direct MP4 streaming (HEAD returned', response.status, ')');
          }
        } catch (error) {
          // Fallback to direct stream on error
          this.movieStreamUrl = directUrl;
          console.error('❌ Error testing HLS endpoint:', error);
          console.log('🎬 Fallback to direct streaming');
        }
        
        this.loading = false;
        console.log('✅ Movie loaded from Google Drive:', movie);
        console.log('📹 Stream URL:', this.movieStreamUrl);
        console.log('🎬 Movie ID:', this.movieId);
      },
      error: (err) => {
        console.error('❌ Error loading movie:', err);
        this.error = 'Failed to load movie from Google Drive. Please make sure you are authenticated.';
        this.loading = false;
      }
    });
  }
  
  private getAuthToken(): string {
    // Get token from localStorage using the correct key
    const appState = this.localStorageService.get(LocalStorageKeys.AppState);
    console.log('🔑 getAuthToken called, appState exists:', !!appState);
    if (appState && appState.accessToken) {
      const token = appState.accessToken;
      console.log('🔑 Token length:', token.length, 'First 20 chars:', token.substring(0, 20));
      return token;
    }
    console.log('🔑 No accessToken found in appState');
    return '';
  }
}
