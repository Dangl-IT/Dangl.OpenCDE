import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrepareDocumentDownloadComponent } from './prepare-document-download.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('PrepareDocumentDownloadComponent', () => {
  let component: PrepareDocumentDownloadComponent;
  let fixture: ComponentFixture<PrepareDocumentDownloadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, PrepareDocumentDownloadComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PrepareDocumentDownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
