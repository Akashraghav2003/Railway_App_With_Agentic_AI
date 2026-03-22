import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-canceltrain',
  templateUrl: './canceltrain.component.html',
  styleUrls: ['./canceltrain.component.css']
})
export class CanceltrainComponent implements OnInit {
  trainId: number | null = null;
  trainName = '';
  message = '';
  success = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    const selectedTrain = localStorage.getItem('selectedTrain');
    if (selectedTrain) {
      const train = JSON.parse(selectedTrain);
      this.trainId = train.trainID;
      this.trainName = train.trainName;
      localStorage.removeItem('selectedTrain');
    }
  }

  cancelTrain(): void {
    if (!this.trainId) return;

    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.delete(`http://localhost:5063/api/Train/CancelTrain/${this.trainId}`, { headers })
      .subscribe({
        next: (response: any) => {
          this.success = true;
          this.message = response.message || 'Train cancelled successfully';
          this.trainId = null;
        },
        error: (error) => {
          this.success = false;
          this.message = error.error?.message || 'Failed to cancel train';
        }
      });
  }
}
