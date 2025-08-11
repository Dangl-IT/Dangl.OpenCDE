import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadStepperComponent } from './download-stepper.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('DownloadStepperComponent', () => {
  let component: DownloadStepperComponent;
  let fixture: ComponentFixture<DownloadStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, DownloadStepperComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
