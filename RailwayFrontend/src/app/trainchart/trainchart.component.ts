import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'trainchart',
  templateUrl: './trainchart.component.html',
  styleUrls: ['./trainchart.component.css']
})
export class TrainchartComponent implements OnInit {
  trains: any[] = [];
  error: string = '';
  message: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any>('http://localhost:5063/api/Train/GetTrains').subscribe({
      next: (response) => {
        if (response.success) {
          this.trains = response.data.map((train: any) => ({
            ...train,
            showClasses: false,
            trainClasses: train.trainClasses.map((cls: any) => ({
              ...cls,
              availableSeats: cls.availableSeats ?? cls.totalSeat
            }))
          }));
          this.message = response.data.length === 0 ? 'No train available for today.' : response.message;
        } else {
          this.message = response.message || 'No train data received.';
        }
      },
      error: (err) => {
        this.error = 'Failed to load train chart.';
        console.error('API Error:', err);
      }
    });
  }

  toggleClasses(train: any): void {
    train.showClasses = !train.showClasses;
  }
}
