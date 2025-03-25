import { BehaviorSubject, Observable } from "rxjs";
import { ChatMessage, ChatState, IChatState } from "./chat-state";
import { SignalRService } from "../signal-r.service";
import { Injectable } from "@angular/core";
import { AuthenticationService } from "../services/authentication.service";
import { AppStateService } from "../shared/app-state/app-state.service";

@Injectable()
export class ChatStateService {
    private chatState: IChatState = new ChatState();
    private chatStateSubject: BehaviorSubject<IChatState> = new BehaviorSubject<IChatState>(this.chatState);
    private chatStateObservable: Observable<IChatState> = this.chatStateSubject.asObservable();

    private username = "";

    constructor(private signalRService: SignalRService, private appStateService: AppStateService) {

        this.appStateService.getAppState().subscribe(appState => {
            this.username = appState.username || "";
        });

        this.signalRService.setOnReceiveMsgListener((message: ChatMessage) => this.addMessageToState({
            ...message,
            avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e",
        }
        ));

        this.signalRService.setOnReceiveAllMessagesListener((messages: ChatMessage[]) => {
            messages.forEach(message => this.addMessageToState({
                ...message,
                isSent: message.sender === this.username, // TODO: fix this, it's not the best way to check if the message is from the 
                avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e",
            }
            ));
        });
    }

    getChatState(): Observable<IChatState> {
        return this.chatStateObservable;
    }

    sendMessage(message: string) {
        this.signalRService.sendMessageToServer(message, this.username || "", () => {
            const sentChatMessage: ChatMessage = {
                id: 0,
                content: message,
                sender: this.username || "",
                timestamp: new Date(),
                avatar: "https://images.unsplash.com/photo-1494790108377-be9c29b29330",
                isSent: true
            }

            this.addMessageToState(sentChatMessage);
        });
    }

    addMessageToState(message: ChatMessage) {
        this.chatState = this.chatState.clone();
        this.chatState.messages.push(message);
        this.chatStateSubject.next(this.chatState);
    }
}
