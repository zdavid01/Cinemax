import { Component, ViewChild, ElementRef, AfterViewInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AppStateService } from '../../shared/app-state/app-state.service';

declare let shaka: any;

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
    private player: any;

    constructor(private readonly appStateService: AppStateService) {
        this.appStateService.getAppState().subscribe((appState) => {
            this.authToken = appState.accessToken || "";
        })
    }

    ngAfterViewInit() {
        this.videoElement = this.videoElementRef.nativeElement;
        
        // Install built-in polyfills to patch browser incompatibilities
        shaka.polyfill.installAll();

        // Check to see if the browser supports the basic APIs Shaka needs
        if (shaka.Player.isBrowserSupported()) {
            this.initPlayer();
        } else {
            console.error('Browser not supported for HLS streaming!');
        }
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['movieUrl'] && !changes['movieUrl'].firstChange && this.player && this.movieUrl) {
            this.loadVideo();
        }
    }

    private initPlayer() {
        // Load the video if URL is already available
        if (this.movieUrl) {
            this.loadVideo();
        }
    }

    private loadVideo() {
        if (!this.movieUrl || !this.videoElement) return;

        console.log('Loading video from Google Drive:', this.movieUrl);
        
        // Check if it's an HLS stream (.m3u8) or single file
        const isHLS = this.movieUrl.includes('.m3u8') || this.movieUrl.includes('/hls/');
        
        if (isHLS) {
            console.log('📺 HLS stream detected - using Shaka Player');
            this.loadHLSStream();
        } else {
            console.log('🎬 Single file detected - using native HTML5 video');
            this.loadDirectStream();
        }
    }

    private loadHLSStream() {
        // Create Shaka Player for HLS
        this.player = new shaka.Player(this.videoElement);

        // Listen for error events
        this.player.addEventListener('error', this.onErrorEvent.bind(this));

        // Add request filter to include JWT token as query parameter for all requests
        this.player.getNetworkingEngine().registerRequestFilter((type: any, request: any) => {
            // Add access_token as query parameter to all HLS requests (playlist and chunks)
            const url = new URL(request.uris[0]);
            url.searchParams.set('access_token', this.authToken);
            request.uris[0] = url.toString();
            console.log('🔐 Added token to HLS request:', url.pathname);
        });
        
        // Build full URL with access token
        const fullUrl = `http://localhost:8005${this.movieUrl}?access_token=${encodeURIComponent(this.authToken)}`;
        console.log('📺 Loading HLS stream:', fullUrl.substring(0, 80) + '...');

        // Load the HLS manifest
        this.player.load(fullUrl).then(() => {
            console.log('✅ HLS video loaded! Chunks will load on-demand for seeking.');
        }).catch((error: any) => {
            this.onError(error);
        });
    }

    private loadDirectStream() {
        // Use native HTML5 video for single files
        // Check if URL is relative or absolute
        let fullUrl = this.movieUrl;
        
        if (this.movieUrl.startsWith('/')) {
            fullUrl = `http://localhost:8005${this.movieUrl}`;
        }
        
        // Add auth token as query parameter
        const urlWithAuth = `${fullUrl}?access_token=${encodeURIComponent(this.authToken)}`;
        
        console.log('Direct stream URL:', urlWithAuth);
        this.videoElement.src = urlWithAuth;
        
        // Add event handlers
        this.videoElement.onloadedmetadata = () => {
            console.log('✅ Video loaded successfully! Range requests enabled for seeking.');
        };
        
        this.videoElement.onerror = (error) => {
            console.error('❌ Error loading video:', error);
            console.error('Failed URL:', urlWithAuth);
            console.error('Video element error code:', this.videoElement.error?.code);
            console.error('Video element error message:', this.videoElement.error?.message);
        };
    }

    private onErrorEvent(event: any) {
        // Extract the shaka.util.Error object from the event
        this.onError(event.detail);
    }

    private onError(error: any) {
        // Log the error
        console.error('Shaka Player Error:', error);
        console.error('Error code', error.code, 'object', error);
    }
}