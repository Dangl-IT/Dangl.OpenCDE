import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageServersModalComponent } from './manage-servers-modal.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppModule } from 'src/app/app.module';

describe('ManageServersModalComponent', () => {
  let component: ManageServersModalComponent;
  let fixture: ComponentFixture<ManageServersModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageServersModalComponent ],
      imports: [SharedTestingModule, AppModule],
      providers: [
        { provide: MAT_DIALOG_DATA, useValue: '' },
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageServersModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
