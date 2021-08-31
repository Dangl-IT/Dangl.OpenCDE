import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetOpencdeServerComponent } from './set-opencde-server.component';

describe('SetOpencdeServerComponent', () => {
  let component: SetOpencdeServerComponent;
  let fixture: ComponentFixture<SetOpencdeServerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SetOpencdeServerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SetOpencdeServerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
