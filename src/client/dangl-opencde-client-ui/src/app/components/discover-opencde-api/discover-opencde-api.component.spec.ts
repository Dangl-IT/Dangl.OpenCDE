import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscoverOpencdeApiComponent } from './discover-opencde-api.component';

describe('DiscoverOpencdeApiComponent', () => {
  let component: DiscoverOpencdeApiComponent;
  let fixture: ComponentFixture<DiscoverOpencdeApiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiscoverOpencdeApiComponent ]
    })
    .compileComponents();
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
