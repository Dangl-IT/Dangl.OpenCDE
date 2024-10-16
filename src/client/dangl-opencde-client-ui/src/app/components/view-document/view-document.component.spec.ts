import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDocumentComponent } from './view-document.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { AppModule } from 'src/app/app.module';

describe('ViewDocumentComponent', () => {
  let component: ViewDocumentComponent;
  let fixture: ComponentFixture<ViewDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewDocumentComponent ],
      imports: [SharedTestingModule, AppModule]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
