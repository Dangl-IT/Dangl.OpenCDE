import { DataSource } from '@angular/cdk/table';
import { DocumentGet } from '../generated/backend-client';
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginationBaseService } from 'ng-lightquery';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DocumentsService
  extends PaginationBaseService<DocumentGet>
  implements DataSource<DocumentGet>
{
  protected http: HttpClient;

  private projectId: string | null = null;

  constructor() {
    const http = inject(HttpClient);

    super(http);
    this.http = http;
  }

  connect(): Observable<DocumentGet[]> {
    return this.paginationResult.pipe(map((r) => r.data));
  }

  disconnect(): void {}

  setProjectId(projectId: string | null): void {
    this.projectId = projectId;
    this.setBaseUrl();
  }

  private setBaseUrl(): void {
    if (this.projectId) {
      this.baseUrl = `/api/projects/${this.projectId}/documents`;
    } else {
      this.baseUrl = '';
    }
  }
}
