import { TestBed } from '@angular/core/testing';

import { DocumentSelectionService } from './document-selection.service';

describe('DocumentSelectionService', () => {
  let service: DocumentSelectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DocumentSelectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
