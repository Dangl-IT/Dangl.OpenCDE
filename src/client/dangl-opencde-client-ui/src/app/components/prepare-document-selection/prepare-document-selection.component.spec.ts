import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrepareDocumentSelectionComponent } from './prepare-document-selection.component';

describe('PrepareDocumentSelectionComponent', () => {
  let component: PrepareDocumentSelectionComponent;
  let fixture: ComponentFixture<PrepareDocumentSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrepareDocumentSelectionComponent ]
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
