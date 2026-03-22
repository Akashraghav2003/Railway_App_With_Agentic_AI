import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private baseUrl = 'http://127.0.0.1:3333';

  constructor(private http: HttpClient) {}

  sendMessage(question: string, threadId: string): Observable<any> {
    const token = localStorage.getItem('authToken');
    
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` })
    });

    return this.http.post(`${this.baseUrl}/graph/async-agent`, 
      { question, thread_id: threadId }, 
      { headers }
    );
  }

  deleteThread(threadId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/checkpoints/thread/${threadId}`);
  }
}