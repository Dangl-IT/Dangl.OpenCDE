import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadStepperComponent } from './upload-stepper.component';

describe('UploadStepperComponent', () => {
  let component: UploadStepperComponent;
  let fixture: ComponentFixture<UploadStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UploadStepperComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
