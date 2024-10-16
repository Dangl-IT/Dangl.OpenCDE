import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadStepperComponent } from './upload-stepper.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { AppModule } from 'src/app/app.module';

describe('UploadStepperComponent', () => {
  let component: UploadStepperComponent;
  let fixture: ComponentFixture<UploadStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UploadStepperComponent],
      imports: [SharedTestingModule, AppModule]
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
