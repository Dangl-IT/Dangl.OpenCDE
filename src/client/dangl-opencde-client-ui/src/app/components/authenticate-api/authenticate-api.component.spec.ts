import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthenticateApiComponent } from './authenticate-api.component';

describe('AuthenticateApiComponent', () => {
  let component: AuthenticateApiComponent;
  let fixture: ComponentFixture<AuthenticateApiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthenticateApiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthenticateApiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
