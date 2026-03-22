import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  userId: number;
  name: string;
  gender: string;
  age: number;
  address: string;
  email: string;
  phone: number;
  userName: string;
  role: string;
}

export interface Reservation {
  reservationId: number;
  userID: number;
  trainId: number;
  travelDate: string;
  classId: number;
  noOfSeats: number;
  pnrNumber: number;
  bookingStatus: string;
  quota: string;
  bankName: string;
  totalFare: number;
  train?: any;
  passenger?: any[];
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5063/RailwayTicketBooking';

  constructor(private http: HttpClient) {}

  getUserById(userId: number): Observable<ApiResponse<User>> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<ApiResponse<User>>(`${this.baseUrl}/GetUserDetailsById/${userId}`, { headers });
  }

  getAllReservationsByUserId(userId: number): Observable<ApiResponse<Reservation[]>> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<ApiResponse<Reservation[]>>(`http://localhost:5063/api/Train/Reservation/${userId}`, { headers });
  }
}