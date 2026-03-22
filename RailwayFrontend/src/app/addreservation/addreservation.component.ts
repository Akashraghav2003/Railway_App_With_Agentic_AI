import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-add-reservation',
  templateUrl: './addreservation.component.html',
  styleUrls: ['./addreservation.component.css']
})
export class AddReservationComponent implements OnInit {
  reservationForm!: FormGroup;
  showSuccessPopup = false;
  reservationData: any = null;
  previousUrl: string = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.reservationForm = this.fb.group({
      userID: [null, Validators.required],
      trainId: [null, Validators.required],
      travelDate: [null, Validators.required],
      classId: [null, Validators.required],
      noOfSeats: [1, [Validators.required, Validators.min(1)]],
      quota: ['', [Validators.required, Validators.maxLength(50)]],
      bankName: ['', Validators.maxLength(100)],
      passenger: this.fb.array([this.createPassenger()])
    });

    const userId = this.getUserIdFromToken();
    if (userId) {
      this.reservationForm.patchValue({ userID: userId });
    }

    this.route.queryParams.subscribe(params => {
      const trainId = +params['trainId'];
      const travelDate = params['date'];
      const classId = +params['classId'];
      this.previousUrl = params['returnUrl'] || '/dashboard';

      if (trainId) this.reservationForm.patchValue({ trainId });
      if (travelDate) this.reservationForm.patchValue({ travelDate });
      if (classId) this.reservationForm.patchValue({ classId });
    });
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

  get passengerControls() {
    return this.reservationForm.get('passenger') as FormArray;
  }

  createPassenger(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      age: [null, [Validators.required, Validators.min(0), Validators.max(120)]],
      gender: ['', [Validators.required]],
      adharCard: [null, [Validators.required]]
    });
  }

  addPassenger() {
    this.passengerControls.push(this.createPassenger());
  }

  removePassenger(index: number) {
    if (this.passengerControls.length > 1) {
      this.passengerControls.removeAt(index);
    }
  }

  onSubmit() {
    if (this.reservationForm.valid) {
      const reservationData = {
        ...this.reservationForm.getRawValue()
      };

      const token = localStorage.getItem('authToken'); // Match AddTrainComponent
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      this.http.post('http://localhost:5063/api/Train/AddReservation', reservationData, { headers }).subscribe({
        next: (res: any) => {
          this.reservationData = { ...reservationData, pnrNumber: res.pnrNumber || Math.floor(Math.random() * 1000000) };
          this.showSuccessPopup = true;
        },
        error: (err) => {
          alert('Error: ' + (err.error?.message || 'Something went wrong.'));
        }
      });
    }
  }

  generatePDF(): void {
    if (!this.reservationData) return;
    
    const doc = new jsPDF();
    
    // Header
    doc.setFontSize(20);
    doc.text('Railway Ticket', 105, 20, { align: 'center' });
    
    // PNR and Status
    doc.setFontSize(14);
    doc.text(`PNR: ${this.reservationData.pnrNumber}`, 20, 40);
    doc.text('Status: Confirmed', 120, 40);
    
    // Train Details
    doc.setFontSize(12);
    doc.text('Train Details:', 20, 60);
    doc.text(`Train ID: ${this.reservationData.trainId}`, 20, 70);
    doc.text(`Travel Date: ${new Date(this.reservationData.travelDate).toLocaleDateString()}`, 20, 80);
    
    // Journey Details
    doc.text('Journey Details:', 20, 100);
    doc.text(`Seats: ${this.reservationData.noOfSeats}`, 20, 110);
    doc.text(`Quota: ${this.reservationData.quota}`, 20, 120);
    doc.text(`Bank: ${this.reservationData.bankName || 'N/A'}`, 20, 130);
    
    // Passenger Details
    doc.text('Passenger Details:', 20, 150);
    let yPos = 160;
    this.reservationData.passenger.forEach((passenger: any, index: number) => {
      doc.text(`${index + 1}. ${passenger.name} (Age: ${passenger.age}, Gender: ${passenger.gender})`, 20, yPos);
      yPos += 10;
    });
    
    // Footer
    doc.setFontSize(10);
    doc.text('Thank you for choosing our railway service!', 105, 280, { align: 'center' });
    
    // Download PDF
    doc.save(`Railway-Ticket-${this.reservationData.pnrNumber}.pdf`);
  }

  closePopupAndRedirect(): void {
    this.showSuccessPopup = false;
    this.router.navigate([this.previousUrl]);
  }

  downloadAndRedirect(): void {
    this.generatePDF();
    setTimeout(() => {
      this.closePopupAndRedirect();
    }, 1000);
  }
}
