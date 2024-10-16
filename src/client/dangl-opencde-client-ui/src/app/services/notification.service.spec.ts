import { TestBed } from '@angular/core/testing';

import { NotificationService } from './notification.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('NotificationService', () => {
  let service: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule]
    });
    service = TestBed.inject(NotificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
