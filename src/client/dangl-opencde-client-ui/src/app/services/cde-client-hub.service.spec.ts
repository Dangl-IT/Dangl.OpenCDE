import { TestBed } from '@angular/core/testing';

import { CdeClientHubService } from './cde-client-hub.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('CdeClientHubService', () => {
  let service: CdeClientHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule]
    });
    service = TestBed.inject(CdeClientHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
