import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiSpecComponent } from './api-spec.component';

describe('ApiSpecComponent', () => {
  let component: ApiSpecComponent;
  let fixture: ComponentFixture<ApiSpecComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApiSpecComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApiSpecComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
