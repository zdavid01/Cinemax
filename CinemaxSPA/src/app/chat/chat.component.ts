import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatStateService } from './chat-state.service';
import { IChatState } from './chat-state';
import { Observable } from 'rxjs';

interface ChatMessage {
  id: number;
  content: string;
  sender: string;
  timestamp: Date;
  avatar: string;
  isSent: boolean;
}

@Component({
  selector: "app-group-chat",
  standalone: true,
  imports: [CommonModule, FormsModule, AsyncPipe],
  templateUrl: "./chat.component.html",
  providers: [ChatStateService]
})
export class ChatComponent {
  @ViewChild("scrollContainer") private scrollContainer!: ElementRef;
  @Input("movieTitle") movieTitle: string = "";

  public chatState$: Observable<IChatState>;

  constructor(private chatService: ChatStateService) {
    this.chatState$ = chatService.getChatState();
  }
  
  messages: ChatMessage[] = [];
  newMessage: string = "";
  isTyping: boolean = false;

  ngOnInit() {
    this.loadInitialMessages();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  loadInitialMessages() {

    this.messages =[]
  }

  sendMessage() {
    if (this.newMessage.trim()) {
      const message: ChatMessage = {
        id: this.messages.length + 1,
        content: this.newMessage,
        sender: "Alice Smith",
        timestamp: new Date(),
        avatar: "https://images.unsplash.com/photo-1494790108377-be9c29b29330",
        isSent: true
      };

      this.chatService.sendMessage(this.newMessage);      
      this.newMessage = "";
      this.scrollToBottom();  
    }
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

}
