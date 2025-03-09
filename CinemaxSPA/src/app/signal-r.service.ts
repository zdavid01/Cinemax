import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ChatMessage } from './chat/chat-state';
import { AppStateService } from './shared/app-state/app-state.service';
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

  public startConnection = (accessToken: string) => {
    if (this.hubConnection) {
      this.hubConnection
        .start()
        .then(() => console.log("SignalR Connection Started"))
        .catch(err => console.error("Error etablishing connection", err));
    }
  }

  constructor(private appStateService: AppStateService) {
    this.appStateService.getAppState().subscribe((appState) => {
      this.hubConnection = new signalR
        .HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("http://localhost:5224/chat-hub", this.getOptions(appState.accessToken || ""))
        .build();
      this.startConnection(appState.accessToken || "");
    })
  }

  public setOnReceiveMsgListener(callback: (message: ChatMessage) => void) {
    if (this.hubConnection) {
      this.onNewMessageListener = callback;

      this.hubConnection.on("ReceiveChatMessage", (content, sender, timestamp) => this.onNewMessageListener({ content, timestamp, sender } as ChatMessage));

    }
  }

  public sendMessageToServer(message: string, username: string, onSentSuccessfully: () => void) {
    if (this.hubConnection) {
      this.hubConnection.invoke("SendChatMessage", message, username).then(onSentSuccessfully);
    }
  }
}
