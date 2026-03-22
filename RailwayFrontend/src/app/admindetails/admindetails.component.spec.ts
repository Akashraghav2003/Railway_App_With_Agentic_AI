import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdmindetailsComponent } from './admindetails.component';

describe('AdmindetailsComponent', () => {
  let component: AdmindetailsComponent;
  let fixture: ComponentFixture<AdmindetailsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AdmindetailsComponent]
    });
    fixture = TestBed.createComponent(AdmindetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
