import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-addtrain',
  templateUrl: './addtrain.component.html'
})
export class AddtrainComponent {
  trainForm: FormGroup;
  message: string = '';
  private apiUrl = 'http://localhost:5063/api/Train/AddTrain';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.trainForm = this.fb.group({
      trainName: ['', [Validators.required, Validators.maxLength(100)]],
      sourceStation: ['', [Validators.required, Validators.maxLength(100)]],
      destinationStation: ['', [Validators.required, Validators.maxLength(100)]],
      departureTime: ['', Validators.required],
      arrivalTime: ['', Validators.required],
      totalSeats: [1, [Validators.required, Validators.min(1)]],
      trainClass: this.fb.array([this.createTrainClass()])
    });
  }

  get trainClass(): FormArray {
    return this.trainForm.get('trainClass') as FormArray;
  }

  createTrainClass(): FormGroup {
    return this.fb.group({
      className: ['', Validators.required],
      totalSeat: [1, [Validators.required, Validators.min(1)]],
      fare: [0, [Validators.required, Validators.min(0)]]
    });
  }

  addTrainClass(): void {
    this.trainClass.push(this.createTrainClass());
  }

  removeTrainClass(index: number): void {
    this.trainClass.removeAt(index);
  }

  onSubmit(): void {
    if (this.trainForm.valid) {
      const token = localStorage.getItem('authToken');

      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      this.http.post<any>(this.apiUrl, this.trainForm.value, { headers }).subscribe({
        next: (res) => this.message = res.message || 'Train added successfully!',
        error: (err) => this.message = 'Error: ' + (err.error?.message || 'Something went wrong')
      });
    } else {
      this.message = 'Please fill all required fields correctly.';
    }
  }
}
