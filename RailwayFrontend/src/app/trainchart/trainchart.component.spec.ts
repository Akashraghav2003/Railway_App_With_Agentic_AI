import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainchartComponent } from './trainchart.component';

describe('TrainchartComponent', () => {
  let component: TrainchartComponent;
  let fixture: ComponentFixture<TrainchartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TrainchartComponent]
    });
    fixture = TestBed.createComponent(TrainchartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
