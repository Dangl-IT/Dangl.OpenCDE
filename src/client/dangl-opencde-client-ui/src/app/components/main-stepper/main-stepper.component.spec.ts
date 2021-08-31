import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainStepperComponent } from './main-stepper.component';

describe('MainStepperComponent', () => {
  let component: MainStepperComponent;
  let fixture: ComponentFixture<MainStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MainStepperComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MainStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
