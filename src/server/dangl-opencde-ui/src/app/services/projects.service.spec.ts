import { TestBed } from '@angular/core/testing';

import { ProjectsService } from './projects.service';
import { SharedTestingModule } from '../shared-tesing.module';

describe('ProjectsService', () => {
  let service: ProjectsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SharedTestingModule],
    });
    service = TestBed.inject(ProjectsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
