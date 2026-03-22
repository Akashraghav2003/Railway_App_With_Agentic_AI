import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CanceltrainComponent } from './canceltrain.component';

describe('CanceltrainComponent', () => {
  let component: CanceltrainComponent;
  let fixture: ComponentFixture<CanceltrainComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CanceltrainComponent]
    });
    fixture = TestBed.createComponent(CanceltrainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
