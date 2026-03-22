import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-searchtrain',
  templateUrl: './searchtrain.component.html',
  styleUrls: ['./searchtrain.component.css']
})
export class SearchtrainComponent {
  source = '';
  destination = '';
  travelDate = '';
  trains: any[] = [];
  searched = false;
  selectedClass: { [trainId: string]: any } = {};

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService
  ) {}

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
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
    if (!this.isLoggedIn) {
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
