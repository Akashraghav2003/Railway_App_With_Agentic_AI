import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor() {}

  isLoggedIn(): boolean {
    const token = localStorage.getItem('authToken');
    return !!token;
  }

  logout(): void {
    localStorage.removeItem('authToken');
    window.history.replaceState(null, '', '/login');
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }
}
