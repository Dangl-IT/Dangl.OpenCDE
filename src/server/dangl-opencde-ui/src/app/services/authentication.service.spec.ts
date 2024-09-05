import { TestBed } from '@angular/core/testing';

import { AuthenticationService } from './authentication.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('AuthenticationService', () => {
  let service: AuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule],
    });
    service = TestBed.inject(AuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
