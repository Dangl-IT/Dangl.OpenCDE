import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadStepperComponent } from './download-stepper.component';
import { AppModule } from 'src/app/app.module';

describe('DownloadStepperComponent', () => {
  let component: DownloadStepperComponent;
  let fixture: ComponentFixture<DownloadStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DownloadStepperComponent],
      imports: [AppModule]
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
