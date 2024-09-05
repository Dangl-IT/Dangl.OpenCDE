import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewDocumentComponent } from './new-document.component';
import { AppModule } from 'src/app/app.module';

describe('NewDocumentComponent', () => {
  let component: NewDocumentComponent;
  let fixture: ComponentFixture<NewDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NewDocumentComponent ],
      implements: [AppModule]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NewDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
