import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadStepperComponent } from './upload-stepper.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('UploadStepperComponent', () => {
  let component: UploadStepperComponent;
  let fixture: ComponentFixture<UploadStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, UploadStepperComponent],
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
