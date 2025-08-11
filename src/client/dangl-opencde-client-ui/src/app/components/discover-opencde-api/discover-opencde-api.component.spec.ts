import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscoverOpencdeApiComponent } from './discover-opencde-api.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { AppModule } from 'src/app/app.module';

describe('DiscoverOpencdeApiComponent', () => {
  let component: DiscoverOpencdeApiComponent;
  let fixture: ComponentFixture<DiscoverOpencdeApiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, AppModule, DiscoverOpencdeApiComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DiscoverOpencdeApiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
