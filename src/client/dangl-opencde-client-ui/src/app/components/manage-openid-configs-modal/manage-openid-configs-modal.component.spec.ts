import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageOpenidConfigsModalComponent } from './manage-openid-configs-modal.component';

describe('ManageOpenidConfigsModalComponent', () => {
  let component: ManageOpenidConfigsModalComponent;
  let fixture: ComponentFixture<ManageOpenidConfigsModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageOpenidConfigsModalComponent ]
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
