import { DataSource } from '@angular/cdk/table';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginationBaseService } from 'ng-lightquery';
import { ProjectGet } from '../generated/backend-client';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ProjectsService
  extends PaginationBaseService<ProjectGet>
  implements DataSource<ProjectGet>
{
  constructor(protected http: HttpClient) {
    super(http);
    this.baseUrl = '/api/projects';
  }

  connect(): Observable<ProjectGet[]> {
    return this.paginationResult.pipe(map((r) => r.data));
  }

  disconnect(): void {}
}
