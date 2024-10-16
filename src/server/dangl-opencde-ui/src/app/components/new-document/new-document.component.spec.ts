import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewDocumentComponent } from './new-document.component';
import { AppModule } from 'src/app/app.module';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { MatIconTestingModule } from '@angular/material/icon/testing';

describe('NewDocumentComponent', () => {
  let component: NewDocumentComponent;
  let fixture: ComponentFixture<NewDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NewDocumentComponent],
      imports: [SharedTestingModule, AppModule, MatIconTestingModule],
    }).compileComponents();
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
