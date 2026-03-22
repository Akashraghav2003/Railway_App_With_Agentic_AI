import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-updatetrain',
  templateUrl: './updatetrain.component.html'
})
export class UpdatetrainComponent implements OnInit {
  trainForm: FormGroup;
  message: string = '';
  private apiUrl = 'http://localhost:5063/api/Train';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.trainForm = this.fb.group({
      trainID: [null, [Validators.required, Validators.min(1)]],
      trainName: ['', Validators.required],
      sourceStation: ['', Validators.required],
      destinationStation: ['', Validators.required],
      departureTime: ['', Validators.required],
      arrivalTime: ['', Validators.required],
      totalSeats: [1, [Validators.required, Validators.min(1)]],
      trainClass: this.fb.array([])
    });
  }

  ngOnInit(): void {
    const selectedTrain = localStorage.getItem('selectedTrain');
    if (selectedTrain) {
      const train = JSON.parse(selectedTrain);
      this.fillTrainForm(train);
      localStorage.removeItem('selectedTrain');
    } else {
      this.addTrainClass(); // Add one class by default
    }
  }

  fillTrainForm(train: any): void {
    this.trainForm.patchValue({
      trainID: train.trainID,
      trainName: train.trainName,
      sourceStation: train.sourceStation,
      destinationStation: train.destinationStation,
      departureTime: new Date(train.departureTime).toISOString().slice(0, 16),
      arrivalTime: new Date(train.arrivalTime).toISOString().slice(0, 16),
      totalSeats: train.totalSeats
    });

    // Clear existing train classes and add from selected train
    this.trainClass.clear();
    if (train.trainClasses && train.trainClasses.length > 0) {
      train.trainClasses.forEach((cls: any) => {
        this.trainClass.push(this.createTrainClass({
          className: cls.className,
          totalSeat: cls.totalSeat,
          fare: cls.fare
        }));
      });
    } else {
      this.addTrainClass();
    }
  }

  get trainClass(): FormArray {
    return this.trainForm.get('trainClass') as FormArray;
  }

  createTrainClass(classData: any = {}): FormGroup {
    return this.fb.group({
      className: [classData.className || '', Validators.required],
      totalSeat: [classData.totalSeat || 1, [Validators.required, Validators.min(1)]],
      fare: [classData.fare || 0, [Validators.required, Validators.min(0)]]
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
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      const formValue = { ...this.trainForm.value };
      const trainID = formValue.trainID;
      delete formValue.trainID;

      this.http.put<any>(`${this.apiUrl}/UpdateTrain?trainID=${trainID}`, formValue, { headers }).subscribe({
        next: (res) => this.message = res.message || 'Train updated successfully!',
        error: (err) => this.message = 'Error: ' + (err.error?.message || 'Something went wrong')
      });
    } else {
      this.message = 'Please fill all required fields correctly.';
    }
  }
}
