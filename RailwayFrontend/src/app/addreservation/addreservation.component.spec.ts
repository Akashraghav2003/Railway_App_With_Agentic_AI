import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddReservationComponent } from './addreservation.component';

describe('AddreservationComponent', () => {
  let component: AddReservationComponent;
  let fixture: ComponentFixture<AddReservationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddReservationComponent]
    });
    fixture = TestBed.createComponent(AddReservationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
