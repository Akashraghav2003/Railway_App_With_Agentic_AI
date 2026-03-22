import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancellationTicketComponent } from './cancellation-ticket.component';

describe('CancellationTicketComponent', () => {
  let component: CancellationTicketComponent;
  let fixture: ComponentFixture<CancellationTicketComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CancellationTicketComponent]
    });
    fixture = TestBed.createComponent(CancellationTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
