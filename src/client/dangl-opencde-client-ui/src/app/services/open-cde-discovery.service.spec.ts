import { TestBed } from '@angular/core/testing';

import { OpenCdeDiscoveryService } from './open-cde-discovery.service';

describe('OpenCdeDiscoveryService', () => {
  let service: OpenCdeDiscoveryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OpenCdeDiscoveryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
