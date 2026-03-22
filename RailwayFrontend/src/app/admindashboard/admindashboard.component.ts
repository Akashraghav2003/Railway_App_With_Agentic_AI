import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-admindashboard',
  templateUrl: './admindashboard.component.html',
  styleUrls: ['./admindashboard.component.css']
})
export class AdmindashboardComponent implements OnInit {
  userId: number | null = null;
  trains: any[] = [];
  loading = false;

  constructor(private authService: AuthService, private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.userId = this.getUserIdFromToken();
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

  updateTrain(trainId: number): void {
    this.router.navigate(['/updatetrain'], { queryParams: { trainId } });
  }

  cancelTrain(trainId: number): void {
    if (confirm('Are you sure you want to cancel this train?')) {
      this.http.delete(`http://localhost:5063/api/Train/CancelTrain/${trainId}`)
        .subscribe({
          next: (response) => {
            alert('Train cancelled successfully');
            this.loadTrains();
          },
          error: (error) => {
            console.error('Error cancelling train:', error);
            alert('Error cancelling train');
          }
        });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
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
