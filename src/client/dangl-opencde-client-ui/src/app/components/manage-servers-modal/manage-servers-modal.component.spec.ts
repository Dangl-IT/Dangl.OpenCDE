import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageServersModalComponent } from './manage-servers-modal.component';

describe('ManageServersModalComponent', () => {
  let component: ManageServersModalComponent;
  let fixture: ComponentFixture<ManageServersModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageServersModalComponent ]
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
