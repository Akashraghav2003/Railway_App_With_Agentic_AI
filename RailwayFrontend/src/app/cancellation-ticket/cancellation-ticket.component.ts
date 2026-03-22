import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-cancellation-ticket',
  templateUrl: './cancellation-ticket.component.html',
  styleUrls: ['./cancellation-ticket.component.css'],
  animations: [
    trigger('fadeSlide', [
      state('visible', style({ opacity: 1, transform: 'translateY(0)' })),
      state('hidden', style({ opacity: 0, transform: 'translateY(-20px)' })),
      transition('hidden => visible', animate('500ms ease-out')),
      transition('visible => hidden', animate('300ms ease-in'))
    ])
  ]
})
export class CancellationTicketComponent {
  pnrNumber: string = '';
  reason: string = '';
  reservationId: string = '';
  responseMessage: string = '';
  animationState = 'visible';

  constructor(private http: HttpClient) {}

  cancelReservation() {
    const payload = {
      pnrNumber: Number(this.pnrNumber),
      reason: this.reason,
      reservationId: Number(this.reservationId)
    };

    this.http.post('http://localhost:5063/api/Train/CancelReservation', payload, { responseType: 'text' })
      .subscribe({
        next: (response) => {
          this.responseMessage = response;
          this.animationState = 'visible';
        },
        error: () => {
          this.responseMessage = 'Cancellation failed. Please try again.';
          this.animationState = 'visible';
        }
      });
  }
}