import { Injectable } from "@angular/core";
import { AppState } from "../shared/app-state/app-state";
import { AppStateService } from "../shared/app-state/app-state.service";
import { AuthenticationService } from "./authentication.service";

@Injectable({
    providedIn: 'root',
})
export class CommunicationService {
    constructor(private readonly appStateService: AppStateService) {
        navigator.serviceWorker.addEventListener("message", async (event) => {
            this.appStateService.getAppState().subscribe((appState) => {
                if (event.data === "requestToken") {
                    const token = appState.accessToken
                    event.ports[0].postMessage(token);
                }
            })
        })
    }
}