import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthenticateApiComponent } from './authenticate-api.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';
import { AppModule } from 'src/app/app.module';

describe('AuthenticateApiComponent', () => {
  let component: AuthenticateApiComponent;
  let fixture: ComponentFixture<AuthenticateApiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthenticateApiComponent ],
      imports: [SharedTestingModule, AppModule]
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
