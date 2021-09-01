import { TestBed } from '@angular/core/testing';

import { JwtRequestValidatorService } from './jwt-request-validator.service';

describe('JwtRequestValidatorService', () => {
  let service: JwtRequestValidatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JwtRequestValidatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
