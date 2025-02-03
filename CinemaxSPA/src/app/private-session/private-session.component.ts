import { Component } from '@angular/core';
import { ChatComponent } from "../chat/chat.component";

@Component({
  selector: 'app-private-session',
  imports: [ChatComponent],
  templateUrl: './private-session.component.html',
  styleUrl: './private-session.component.css'
})
export class PrivateSessionComponent {

}
