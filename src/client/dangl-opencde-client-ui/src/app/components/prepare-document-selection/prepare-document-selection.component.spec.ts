import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrepareDocumentSelectionComponent } from './prepare-document-selection.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('PrepareDocumentSelectionComponent', () => {
  let component: PrepareDocumentSelectionComponent;
  let fixture: ComponentFixture<PrepareDocumentSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrepareDocumentSelectionComponent ],
      imports: [SharedTestingModule]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PrepareDocumentSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
