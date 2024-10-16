import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageOpenidConfigsModalComponent } from './manage-openid-configs-modal.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { AppModule } from 'src/app/app.module';

describe('ManageOpenidConfigsModalComponent', () => {
  let component: ManageOpenidConfigsModalComponent;
  let fixture: ComponentFixture<ManageOpenidConfigsModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageOpenidConfigsModalComponent ],
      imports: [SharedTestingModule, AppModule]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageOpenidConfigsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
