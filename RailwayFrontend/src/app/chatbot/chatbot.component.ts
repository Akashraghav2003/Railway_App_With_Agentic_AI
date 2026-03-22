import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ChatbotService } from '../services/chatbot.service';

interface Message {
  content: string;
  isUser: boolean;
  timestamp: Date;
}

@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent {
  isOpen = false;
  messages: Message[] = [];
  currentMessage = '';
  isLoading = false;
  threadId = this.generateThreadId();

  constructor(private chatbotService: ChatbotService, private authService: AuthService) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
    if (this.isOpen && this.messages.length === 0) {
      const userName = this.getUserName();
      const greeting = userName ? 
        `Hello ${userName}! I'm your Railway Assistant. How can I help you today?` :
        'Hello! I\'m your Railway Assistant. How can I help you today?';
      this.addMessage(greeting, false);
    }
  }

  async sendMessage() {
    if (!this.currentMessage.trim() || this.isLoading) return;

    const userMessage = this.currentMessage.trim();
    this.addMessage(userMessage, true);
    this.currentMessage = '';
    this.isLoading = true;

    try {
      const token = this.authService.getToken();
      console.log('Sending message with token:', token ? 'Token present' : 'No token');
      
      const response = await this.chatbotService.sendMessage(
        userMessage, 
        this.threadId
      ).toPromise();

      // Format the response content for better display
      const formattedContent = this.formatResponse(response);
      this.addMessage(formattedContent, false);
    } catch (error: any) {
      console.error('Chat error:', error);
      const errorMsg = error.error?.detail || error.message || 'Sorry, I encountered an error. Please try again.';
      this.addMessage(errorMsg, false);
    } finally {
      this.isLoading = false;
    }
  }

  private addMessage(content: string, isUser: boolean) {
    this.messages.push({
      content,
      isUser,
      timestamp: new Date()
    });
  }

  private formatResponse(response: any): string {
    try {
      // If response has content property, use it
      if (response.content) {
        return this.cleanAndFormatText(response.content);
      }
      
      // If response is a string, use it directly
      if (typeof response === 'string') {
        return this.cleanAndFormatText(response);
      }
      
      // If response has data or result property
      if (response.data) {
        return this.formatDataResponse(response.data);
      }
      
      // If response has message property
      if (response.message) {
        return this.cleanAndFormatText(response.message);
      }
      
      // For complex objects, try to extract meaningful information
      if (typeof response === 'object') {
        return this.formatObjectResponse(response);
      }
      
      return 'Response received successfully.';
    } catch (error) {
      console.error('Error formatting response:', error);
      return 'I received your request but had trouble formatting the response.';
    }
  }
  
  private cleanAndFormatText(text: string): string {
    // Remove extra whitespace and format the text
    return text
      .replace(/\n\s*\n/g, '\n\n') // Clean up multiple newlines
      .replace(/\s+/g, ' ') // Replace multiple spaces with single space
      .trim();
  }
  
  private formatDataResponse(data: any): string {
    if (Array.isArray(data)) {
      if (data.length === 0) {
        return 'No results found.';
      }
      
      // Format array data (like train search results)
      let formatted = `Found ${data.length} result(s):\n\n`;
      data.slice(0, 5).forEach((item, index) => { // Show max 5 items
        formatted += `${index + 1}. ${this.formatSingleItem(item)}\n`;
      });
      
      if (data.length > 5) {
        formatted += `\n... and ${data.length - 5} more results.`;
      }
      
      return formatted;
    }
    
    return this.formatSingleItem(data);
  }
  
  private formatSingleItem(item: any): string {
    if (typeof item === 'string') {
      return item;
    }
    
    if (typeof item === 'object' && item !== null) {
      // Format common railway-related objects
      if (item.trainName || item.trainId) {
        return `Train: ${item.trainName || item.trainId} | From: ${item.source || 'N/A'} | To: ${item.destination || 'N/A'} | Time: ${item.departureTime || 'N/A'}`;
      }
      
      if (item.reservationId) {
        return `Reservation ID: ${item.reservationId} | Status: ${item.status || 'N/A'} | Date: ${item.date || 'N/A'}`;
      }
      
      // Generic object formatting
      const keyValuePairs = Object.entries(item)
        .filter(([key, value]) => value !== null && value !== undefined)
        .map(([key, value]) => `${key}: ${value}`)
        .slice(0, 3); // Show max 3 properties
      
      return keyValuePairs.join(' | ');
    }
    
    return String(item);
  }
  
  private formatObjectResponse(obj: any): string {
    // Handle success/error responses
    if (obj.success !== undefined) {
      const status = obj.success ? '✅ Success' : '❌ Failed';
      const message = obj.message || obj.error || '';
      return `${status}${message ? ': ' + message : ''}`;
    }
    
    // Handle status responses
    if (obj.status) {
      return `Status: ${obj.status}${obj.message ? ' - ' + obj.message : ''}`;
    }
    
    // Try to extract meaningful text from the object
    const meaningfulKeys = ['message', 'result', 'response', 'data', 'content', 'text'];
    for (const key of meaningfulKeys) {
      if (obj[key]) {
        return this.cleanAndFormatText(String(obj[key]));
      }
    }
    
    // Last resort: show key-value pairs
    const entries = Object.entries(obj)
      .filter(([key, value]) => value !== null && value !== undefined)
      .slice(0, 3);
    
    if (entries.length > 0) {
      return entries.map(([key, value]) => `${key}: ${value}`).join('\n');
    }
    
    return 'Response received.';
  }
  
  private generateThreadId(): string {
    return 'thread_' + Math.random().toString(36).substr(2, 9);
  }

  private getUserName(): string | null {
    try {
      const token = this.authService.getToken();
      if (token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.name || payload.username || payload.sub || null;
      }
    } catch (error) {
      console.log('Could not extract user name from token');
    }
    return null;
  }

  formatMessageForDisplay(content: string): string {
    // Convert numbered lists to bullet points with proper paragraph spacing
    let formatted = content
      .replace(/\d+\. /g, '<br><br>• ') // Replace "1. " with paragraph break and bullet
      .replace(/^<br><br>/, '') // Remove leading breaks
      .replace(/\n/g, '<br>')
      .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>') // Bold text
      .replace(/\*(.*?)\*/g, '<em>$1</em>') // Italic text
      .replace(/(✅|❌|🚂|🎫|📅|⏰|🚉)/g, '<span class="emoji">$1</span>') // Emojis
      .replace(/(Train:|From:|To:|Time:|Status:|Date:|Reservation ID:)/g, '<span class="label">$1</span>'); // Labels
    
    return formatted;
  }
  
  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  async deleteCurrentChat() {
    if (confirm('Are you sure you want to delete this chat? This action cannot be undone.')) {
      try {
        await this.chatbotService.deleteThread(this.threadId).toPromise();
        this.startNewChat();
      } catch (error) {
        console.error('Error deleting chat:', error);
      }
    }
  }

  startNewChat() {
    this.messages = [];
    this.threadId = this.generateThreadId();
    this.currentMessage = '';
    
    const userName = this.getUserName();
    const greeting = userName ? 
      `Hello ${userName}! I'm your Railway Assistant. How can I help you today?` :
      'Hello! I\'m your Railway Assistant. How can I help you today?';
    this.addMessage(greeting, false);
  }
}