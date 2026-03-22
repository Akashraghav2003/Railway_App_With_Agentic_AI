import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserService, User } from '../services/user.service';

@Component({
  selector: 'app-admindetails',
  templateUrl: './admindetails.component.html',
  styleUrls: ['./admindetails.component.css']
})
export class AdmindetailsComponent implements OnInit {
  user: User | null = null;
  trains: any[] = [];
  loading = false;
  error = '';

  constructor(private userService: UserService, private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadUserDetails();
    this.loadTrains();
  }

  loadTrains(): void {
    this.loading = true;
    this.http.get<any>('http://localhost:5063/api/Train/GetAllTrains')
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.trains = response.data;
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading trains:', error);
          this.loading = false;
        }
      });
  }

  updateTrain(train: any): void {
    localStorage.setItem('selectedTrain', JSON.stringify(train));
    this.router.navigate(['/updatetrain']);
  }

  cancelTrain(train: any): void {
    localStorage.setItem('selectedTrain', JSON.stringify(train));
    this.router.navigate(['/canceltrain']);
  }

  loadUserDetails(): void {
    const userId = this.getUserIdFromToken();
    if (userId) {
      this.loading = true;
      this.userService.getUserById(userId).subscribe({
        next: (response) => {
          if (response.success) {
            this.user = response.data;
          } else {
            this.error = response.message;
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load admin details';
          this.loading = false;
        }
      });
    }
  }

  private getUserIdFromToken(): number | null {
    const token = localStorage.getItem('authToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.UserId);
      } catch (e) {
        return null;
      }
    }
    return null;
  }
}
