import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentSelectionStartComponent } from './document-selection-start.component';
import { AppModule } from 'src/app/app.module';

describe('DocumentSelectionStartComponent', () => {
  let component: DocumentSelectionStartComponent;
  let fixture: ComponentFixture<DocumentSelectionStartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentSelectionStartComponent ],
      imports: [AppModule]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentSelectionStartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
