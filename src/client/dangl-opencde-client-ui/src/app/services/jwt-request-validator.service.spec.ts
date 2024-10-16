import { TestBed } from '@angular/core/testing';

import { JwtRequestValidatorService } from './jwt-request-validator.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('JwtRequestValidatorService', () => {
  let service: JwtRequestValidatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule]
    });
    service = TestBed.inject(JwtRequestValidatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
