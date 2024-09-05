import { TestBed } from '@angular/core/testing';

import { DocumentsService } from './documents.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('DocumentsService', () => {
  let service: DocumentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule],
    });
    service = TestBed.inject(DocumentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
