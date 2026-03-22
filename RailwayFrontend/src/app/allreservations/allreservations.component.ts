import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import jsPDF from 'jspdf';

interface Passenger {
  passengerId: number;
  name: string;
  age: number;
  gender: string;
  adharCard: number;
}

interface Reservation {
  reservationId: number;
  trainName: string;
  trainId: number;
  source: string;
  destination: string;
  travelDate: string;
  noOfSeats: number;
  pnrNumber: number;
  bookingStatus: string;
  totalFare: number;
  bankName: string;
  passengers: Passenger[];
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Component({
  selector: 'app-allreservations',
  templateUrl: './allreservations.component.html',
  styleUrls: ['./allreservations.component.css']
})
export class AllreservationsComponent implements OnInit {
  reservations: Reservation[] = [];
  loading = false;
  error = '';
  showSuccessPopup = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(): void {
    const userId = this.getUserIdFromToken();
    if (userId) {
      this.loading = true;
      const token = localStorage.getItem('authToken');
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

      this.http.get<ApiResponse<Reservation[]>>(
        `http://localhost:5063/RailwayTicketBooking/GetAllReservationsByUserId/${userId}`,
        { headers }
      ).subscribe({
        next: (response) => {
          if (response.success) {
            this.reservations = response.data;
          } else {
            this.error = response.message;
          }
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load reservations';
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
      } catch {
        return null;
      }
    }
    return null;
  }

  generatePDF(reservation: Reservation): void {
    const doc = new jsPDF();
    
    // Header with logo space
    doc.setFillColor(40, 167, 69);
    doc.rect(0, 0, 210, 30, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(24);
    doc.text('🚂 RAILWAY TICKET', 105, 20, { align: 'center' });
    
    // Reset text color
    doc.setTextColor(0, 0, 0);
    
    // Ticket Information Box
    doc.setDrawColor(40, 167, 69);
    doc.setLineWidth(2);
    doc.rect(15, 40, 180, 40);
    
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('TICKET INFORMATION', 20, 50);
    
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(12);
    doc.text(`Reservation ID: ${reservation.reservationId}`, 20, 60);
    doc.text(`PNR Number: ${reservation.pnrNumber}`, 110, 60);
    doc.text(`Train ID: ${reservation.trainId}`, 20, 70);
    doc.text(`Status: ${reservation.bookingStatus}`, 110, 70);
    
    // Journey Details Box
    doc.rect(15, 90, 180, 30);
    doc.setFont('helvetica', 'bold');
    doc.text('JOURNEY DETAILS', 20, 100);
    
    doc.setFont('helvetica', 'normal');
    doc.text(`Travel Date: ${new Date(reservation.travelDate).toLocaleDateString()}`, 20, 110);
    doc.text(`Seats: ${reservation.noOfSeats}`, 110, 110);
    doc.text(`Total Fare: ₹${reservation.totalFare}`, 20, 120);
    doc.text(`Bank: ${reservation.bankName || 'N/A'}`, 110, 120);
    
    // Passenger Details Table
    doc.setFont('helvetica', 'bold');
    doc.text('PASSENGER DETAILS', 20, 140);
    
    // Table headers
    const tableY = 150;
    doc.setFillColor(240, 240, 240);
    doc.rect(15, tableY - 5, 180, 10, 'F');
    
    doc.setFontSize(10);
    doc.text('S.No', 20, tableY);
    doc.text('Name', 40, tableY);
    doc.text('Age', 90, tableY);
    doc.text('Gender', 110, tableY);
    doc.text('Aadhar Card', 140, tableY);
    
    // Table data
    doc.setFont('helvetica', 'normal');
    let yPos = tableY + 15;
    
    reservation.passengers.forEach((passenger, index) => {
      if (index % 2 === 0) {
        doc.setFillColor(250, 250, 250);
        doc.rect(15, yPos - 5, 180, 10, 'F');
      }
      
      doc.text(`${index + 1}`, 20, yPos);
      doc.text(passenger.name, 40, yPos);
      doc.text(`${passenger.age}`, 90, yPos);
      doc.text(passenger.gender, 110, yPos);
      doc.text(`${passenger.adharCard}`, 140, yPos);
      yPos += 12;
    });
    
    // Footer
    doc.setFillColor(40, 167, 69);
    doc.rect(0, 270, 210, 27, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(10);
    doc.text('Thank you for choosing Railway Management System!', 105, 280, { align: 'center' });
    doc.text(`Generated on: ${new Date().toLocaleString()}`, 105, 290, { align: 'center' });
    
    doc.save(`Railway-Ticket-${reservation.pnrNumber}.pdf`);
    this.showSuccessPopup = true;
    setTimeout(() => this.showSuccessPopup = false, 3000);
  }

  cancelReservation(pnrNumber: number): void {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.http.post(`http://localhost:5063/api/Train/CancelReservation`, { pnrNumber }, { headers }).subscribe({
      next: () => this.loadReservations(),
      error: () => alert('Cancellation failed')
    });
  }

  closePopup(): void {
    this.showSuccessPopup = false;
  }
}