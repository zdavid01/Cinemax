import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ChatMessage } from './chat/chat-state';
import { AppStateService } from './shared/app-state/app-state.service';
import { LocalStorageService } from './shared/local-storage/local-storage.service';
import { LocalStorageKeys } from './shared/local-storage/local-storage-keys';


type MessageFromServer = {
  username: string,
  date: string,
  message: string
}

const mapMessageFromServerToChatMessage = (message: MessageFromServer): ChatMessage => {
  return {
    id: 0,
    content: message.message,
    sender: message.username,
    timestamp: new Date(message.date),
    avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e",
    isSent: false
  }
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private getOptions = (accessToken: string): signalR.IHttpConnectionOptions => {
    return {
      accessTokenFactory: () => {
        return accessToken
      }
    }
  }


  private hubConnection: signalR.HubConnection | null = null;

  private onNewMessageListener: (message: ChatMessage) => void = () => {
    throw Error("Message handler not set");
  };

  public startConnection = () => {
    if (this.hubConnection) {
      this.hubConnection
        .start()
        .then(() => {
          this.hubConnection?.invoke("JoinGroup", this.localStorageService.get(LocalStorageKeys.SessionId));
        })
        .catch(err => console.error("Error etablishing connection", err));
    }
  }

  constructor(private appStateService: AppStateService, private localStorageService: LocalStorageService) {
    this.appStateService.getAppState().subscribe((appState) => {
      this.hubConnection = new signalR
        .HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("http://localhost:8005/chat-hub", this.getOptions(appState.accessToken || ""))
        .build();
      this.startConnection();
    })
  }

  public setOnReceiveMsgListener(callback: (message: ChatMessage) => void) {
    if (this.hubConnection) {
      this.onNewMessageListener = callback;

      this.hubConnection.on("ReceiveChatMessage", (content, sender, timestamp) => this.onNewMessageListener({ content, timestamp, sender } as ChatMessage));

    }
  }

  public setOnReceiveAllMessagesListener(callback: (messages: ChatMessage[]) => void) {
    if (this.hubConnection) {

      this.hubConnection.on("ReceiveAllMessages", (messages) => {
        const chatMessages = messages.map(mapMessageFromServerToChatMessage)
        callback(chatMessages)
      });
    }
  }

  public sendMessageToServer(message: string, username: string, onSentSuccessfully: () => void) {
    if (this.hubConnection) {
      const sessionId = this.localStorageService.get(LocalStorageKeys.SessionId);
      this.hubConnection.invoke("SendChatMessage", sessionId, message, username).then(onSentSuccessfully);
    }
  }
}
