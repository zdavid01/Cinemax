import { Component, OnInit } from '@angular/core';
import { ChatComponent } from "../chat/chat.component";
import { IMovieForStreaming, MovieStreamingService } from '../services/movie-streaming.service';
import { AsyncPipe } from '@angular/common';
import { VideoPlayerComponent } from "./video-player";
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-private-session',
  imports: [ChatComponent, AsyncPipe, VideoPlayerComponent],
  templateUrl: './private-session.component.html',
})
export class PrivateSessionComponent implements OnInit {

  public movieForStreaming: IMovieForStreaming | null = null;
  public error: string = "";
  public movieStreamUrl = "";
  public movieId: string = "";

  constructor(private movieStreamingService: MovieStreamingService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.movieId = params["sessionId"];
    });

    this.movieStreamingService.getMovieMetadata(this.movieId).subscribe((movie: IMovieForStreaming) => {
      this.movieForStreaming = movie;
      this.movieStreamUrl = `http://localhost:5224/Movie/stream/${this.movieId}`;
    });
  }
}
