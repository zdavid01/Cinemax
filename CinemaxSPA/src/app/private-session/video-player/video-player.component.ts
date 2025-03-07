import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, Input } from '@angular/core';
import { AppStateService } from '../../shared/app-state/app-state.service';

declare let shaka: any;

@Component({
    selector: 'app-video-player',
    templateUrl: './video-player.component.html',
})
export class VideoPlayerComponent implements AfterViewInit {
    @ViewChild('videoPlayer') videoElementRef!: ElementRef;
    @Input("movieUrl") movieUrl: string = "";
    @Input("streamId") streamId: string = "1";

    videoElement!: HTMLVideoElement;

    private authToken = "";

    constructor(private readonly appStateService: AppStateService) {
        this.appStateService.getAppState().subscribe((appState) => {
            this.authToken = appState.accessToken || "";
        })
    }

    ngAfterViewInit() {
        // Install built-in polyfills to patch browser incompatibilities.
        shaka.polyfill.installAll();

        // Check to see if the browser supports the basic APIs Shaka needs.
        if (shaka.Player.isBrowserSupported()) {
            // Everything looks good!
            this.videoElement = this.videoElementRef.nativeElement;
            this.initPlayer();
        } else {
            // This browser does not have the minimum set of APIs we need.
            console.error('Browser not supported!');
        }
    }

    private initPlayer() {
        // Create a Player instance.
        // var video = document.getElementById('video');
        let player = new shaka.Player(this.videoElement);

        // Attach player to the window to make it easy to access in the JS console.
        // window.player = player;

        // Listen for error events.
        player.addEventListener('error', this.onErrorEvent);

        player.getNetworkingEngine().registerRequestFilter((type: any, request: any) => {
            if (type === shaka.net.NetworkingEngine.RequestType.LICENSE) {
                // license-specific headers
            } else {
                request.headers["Authorization"] = `Bearer ${this.authToken}`;
                const { uris } = request;
                if (uris[0].endsWith('ts')) {
                    const uri = uris[0];
                    const base = uri.slice(0, uri.lastIndexOf("/"))
                    const filename = uri.slice(uri.lastIndexOf("/") + 1)
                    const newUri = `${base}/${this.streamId}/${filename}`;
                    uris[0] = newUri;
                }
            }
        })

        // // Try to load a manifest.
        // // This is an asynchronous process.
        player.load(this.movieUrl).then(() => {
            //     // This runs if the asynchronous load is successful.
            console.log('The video has now been loaded!');
        }).catch(this.onError);  // onError is executed if the asynchronous load fails.
    }

    private onErrorEvent(event: any) {
        // Extract the shaka.util.Error object from the event.
        this.onError(event.detail);
    }

    private onError(error: any) {
        // Log the error.
        console.error('Error code', error.code, 'object', error);
    }

}