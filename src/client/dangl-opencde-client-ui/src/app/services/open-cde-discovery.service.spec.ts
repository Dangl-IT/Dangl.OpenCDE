import { TestBed } from '@angular/core/testing';

import { OpenCdeDiscoveryService } from './open-cde-discovery.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('OpenCdeDiscoveryService', () => {
  let service: OpenCdeDiscoveryService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule]
    });
    service = TestBed.inject(OpenCdeDiscoveryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
