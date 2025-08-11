import { AppComponent } from './app.component';
import { TestBed } from '@angular/core/testing';
import { SharedTestingModule } from './shared-tesing.module';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, AppComponent],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'dangl-opencde-client-ui'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('dangl-opencde-client-ui');
  });
});
