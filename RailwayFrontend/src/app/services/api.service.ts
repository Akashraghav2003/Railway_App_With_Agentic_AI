import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5063';

  constructor(private http: HttpClient) {}

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/RailwayTicketBooking/ForgetPassword?email=${email}`, {});
  }

  resetPassword(resetData: any, token: string): Observable<any> {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    };
    return this.http.post(`${this.baseUrl}/RailwayTicketBooking/ResetPassword`, resetData, { headers });
  }
}