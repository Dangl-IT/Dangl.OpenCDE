import { TestBed } from '@angular/core/testing';

import { CdeSessionService } from './cde-session.service';

describe('CdeSessionService', () => {
  let service: CdeSessionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CdeSessionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
