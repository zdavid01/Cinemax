import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild } from '@angular/core';
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
  styleUrls: ["./chat.component.css"],
  providers: [ChatStateService]
})
export class ChatComponent {
  @ViewChild("scrollContainer") private scrollContainer!: ElementRef;

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
    this.messages = [
      {
        id: 1,
        content: "Hey team! How's the new feature coming along?",
        sender: "John Doe",
        timestamp: new Date(),
        avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e",
        isSent: false
      },
      {
        id: 2,
        content: "Making good progress! The core functionality is implemented.",
        sender: "Alice Smith",
        timestamp: new Date(),
        avatar: "https://images.unsplash.com/photo-1494790108377-be9c29b29330",
        isSent: true
      }
    ];

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
      // this.messages.push(message);
      this.newMessage = "";
      this.scrollToBottom();

      // Simulate receiving a response
      // this.simulateResponse();
      
    }
  }

  simulateResponse() {
    this.isTyping = true;
    setTimeout(() => {
      this.isTyping = false;
      const response: ChatMessage = {
        id: this.messages.length + 1,
        content: "Thanks for the update! Let me know if you need any help.",
        sender: "John Doe",
        timestamp: new Date(),
        avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e",
        isSent: false
      };
      this.messages.push(response);
    }, 2000);
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

}
