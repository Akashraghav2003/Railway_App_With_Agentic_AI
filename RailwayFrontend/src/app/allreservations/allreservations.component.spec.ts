import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllreservationsComponent } from './allreservations.component';

describe('AllreservationsComponent', () => {
  let component: AllreservationsComponent;
  let fixture: ComponentFixture<AllreservationsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AllreservationsComponent]
    });
    fixture = TestBed.createComponent(AllreservationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
