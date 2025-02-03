export interface ChatMessage {
    id: number;
    content: string;
    sender: string;
    timestamp: Date;
    avatar: string;
    isSent: boolean;
}

export interface IChatState  {
    messages: ChatMessage[],
    eventId: string;
    clone: () => IChatState
}

export class ChatState implements IChatState {
    public messages: ChatMessage[]
    public eventId = "";
    clone() {
        const chatState = new ChatState();
        chatState.eventId = this.eventId;
        chatState.messages = [
            ...this.messages
        ]

        return chatState;
    }
    constructor() {
        this.messages = []
        this.eventId = ""
    }
}
