import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserService, User } from '../services/user.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-userdetails',
  templateUrl: './userdetails.component.html',
  styleUrls: ['./userdetails.component.css']
})
export class UserdetailsComponent implements OnInit {
  user: User | null = null;
  loading = false;
  error = '';
  source = '';
  destination = '';
  travelDate = '';
  trains: any[] = [];
  searched = false;

  constructor(private userService: UserService, private authService: AuthService, private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadUserDetails();
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
          this.error = 'Failed to load user details';
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

  getCurrentDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  searchTrains() {
    if (!this.source || !this.destination || !this.travelDate) {
      alert('Please fill in all fields.');
      return;
    }

    if (new Date(this.travelDate) < new Date(this.getCurrentDate())) {
      alert('Cannot search trains for past dates.');
      return;
    }

    const selectedDate = new Date(this.travelDate).toISOString().split('T')[0];

    this.http.get<any>(`http://localhost:5063/api/Train/search?source=${this.source}&destination=${this.destination}&date=${this.travelDate}`)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.trains = response.data.map((train: any) => ({
              ...train,
              trainId: train.trainID,
              classes: train.trainClasses.map((cls: any) => ({
                classId: cls.classId,
                name: cls.className,
                fare: cls.fare,
                availableSeats: cls.totalSeat
              }))
            }));
          } else {
            this.trains = [];
          }
          this.searched = true;
        },
        error: (error) => {
          console.error('Error fetching trains:', error);
          this.trains = [];
          this.searched = true;
        }
      });
  }

  bookTrainWithClass(train: any, selectedClass: any) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/addreservation'], {
        queryParams: {
          trainId: train.trainId,
          classId: selectedClass.classId,
          date: this.travelDate
        }
      });
    }
  }
}
