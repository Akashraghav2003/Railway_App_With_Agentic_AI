import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserslidebarComponent } from './userslidebar.component';

describe('UserslidebarComponent', () => {
  let component: UserslidebarComponent;
  let fixture: ComponentFixture<UserslidebarComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UserslidebarComponent]
    });
    fixture = TestBed.createComponent(UserslidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
