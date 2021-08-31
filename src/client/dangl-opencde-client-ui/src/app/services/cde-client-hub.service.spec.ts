import { TestBed } from '@angular/core/testing';

import { CdeClientHubService } from './cde-client-hub.service';

describe('CdeClientHubService', () => {
  let service: CdeClientHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CdeClientHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
