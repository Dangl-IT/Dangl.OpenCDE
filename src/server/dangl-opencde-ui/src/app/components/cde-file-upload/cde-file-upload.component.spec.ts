import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CdeFileUploadComponent } from './cde-file-upload.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('CdeFileUploadComponent', () => {
  let component: CdeFileUploadComponent;
  let fixture: ComponentFixture<CdeFileUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, CdeFileUploadComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CdeFileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
