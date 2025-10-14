import { Component, ViewChild, ElementRef, AfterViewInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AppStateService } from '../../shared/app-state/app-state.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
    selector: 'app-video-player',
    templateUrl: './video-player.component.html',
})
export class VideoPlayerComponent implements AfterViewInit, OnChanges {
    @ViewChild('videoPlayer') videoElementRef!: ElementRef;
    @Input("movieUrl") movieUrl: string = "";
    @Input("streamId") streamId: string = "1";

    videoElement!: HTMLVideoElement;
    private authToken = "";

    constructor(
        private readonly appStateService: AppStateService,
        private http: HttpClient
    ) {
        this.appStateService.getAppState().subscribe((appState) => {
            this.authToken = appState.accessToken || "";
        })
    }

    ngAfterViewInit() {
        this.videoElement = this.videoElementRef.nativeElement;
        if (this.movieUrl) {
            this.loadVideo();
        }
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['movieUrl'] && !changes['movieUrl'].firstChange && this.videoElement) {
            this.loadVideo();
        }
    }

    private loadVideo() {
        if (!this.movieUrl || !this.videoElement) return;

        console.log('Loading video from Google Drive via backend proxy:', this.movieUrl);
        
        // Since Google Drive has CORS restrictions, we need to add the auth token
        // as a query parameter so the backend can authenticate the request
        const urlWithAuth = `${this.movieUrl}?access_token=${encodeURIComponent(this.authToken)}`;
        
        // Set the video source directly - the backend will proxy/redirect to Google Drive
        this.videoElement.src = urlWithAuth;
        
        // Add error handler
        this.videoElement.onerror = (error) => {
            console.error('Error loading video:', error);
            console.log('Video URL:', urlWithAuth);
        };

        // Add loaded handler
        this.videoElement.onloadedmetadata = () => {
            console.log('Video metadata loaded successfully');
        };
    }
}