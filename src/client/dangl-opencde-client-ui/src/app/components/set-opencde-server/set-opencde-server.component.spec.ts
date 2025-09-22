import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetOpencdeServerComponent } from './set-opencde-server.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('SetOpencdeServerComponent', () => {
  let component: SetOpencdeServerComponent;
  let fixture: ComponentFixture<SetOpencdeServerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, SetOpencdeServerComponent],
    }).compileComponents();
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
