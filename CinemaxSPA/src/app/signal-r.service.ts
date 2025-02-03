import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ChatMessage } from './chat/chat-state';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection = new signalR
  .HubConnectionBuilder()
  .withUrl("http://localhost:5224/chat-hub")
  .build();

  private onNewMessageListener: (message: ChatMessage) => void = () => {
    throw Error("Message handler not set");
  };

  public startConnection = () => {
    this.hubConnection
      .start()
      .then(() => console.log("SignalR Connection Started"))
      .catch(err => console.error("Error etablishing connection", err));
  }
  constructor() {
    this.startConnection();
  }

  public setOnReceiveMsgListener(callback: (message: ChatMessage) => void) {
    this.onNewMessageListener = callback;

    this.hubConnection.on("ReceiveChatMessage", (content, sender, timestamp) => this.onNewMessageListener({content, timestamp, sender} as ChatMessage));
  }

  public sendMessageToServer(message: string, username: string, onSentSuccessfully: ()=>void) {
    this.hubConnection.invoke("SendChatMessage", message, username).then(onSentSuccessfully);
  }
}
