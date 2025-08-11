import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSort, Sort, MatSortHeader } from '@angular/material/sort';
import { Subject, of } from 'rxjs';
import {
  delay,
  distinctUntilChanged,
  switchMap,
  takeUntil,
} from 'rxjs/operators';

import { ActivatedRoute, RouterLink } from '@angular/router';
import { DocumentGet } from '../../generated/backend-client';
import { DocumentsService } from '../../services/documents.service';
import { PageEvent, MatPaginator } from '@angular/material/paginator';
import { PaginationResult } from 'ng-lightquery';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import {
  MatTable,
  MatColumnDef,
  MatHeaderCellDef,
  MatHeaderCell,
  MatCellDef,
  MatCell,
  MatHeaderRowDef,
  MatHeaderRow,
  MatRowDef,
  MatRow,
} from '@angular/material/table';
import { NgIf, DatePipe } from '@angular/common';
import { FileSizePipe } from '../../pipes/file-size.pipe';

@Component({
  selector: 'opencde-documents',
  templateUrl: './documents.component.html',
  styleUrls: ['./documents.component.scss'],
  imports: [
    MatFormField,
    MatInput,
    FormsModule,
    MatButton,
    RouterLink,
    MatTable,
    MatSort,
    MatColumnDef,
    MatHeaderCellDef,
    MatHeaderCell,
    MatSortHeader,
    MatCellDef,
    MatCell,
    NgIf,
    MatHeaderRowDef,
    MatHeaderRow,
    MatRowDef,
    MatRow,
    MatPaginator,
    DatePipe,
    FileSizePipe,
  ],
})
export class DocumentsComponent implements OnInit, OnDestroy {
  private unsubscribe: Subject<void> = new Subject<void>();
  private projectId: string | null = null;
  @ViewChild(MatSort, { static: true }) private sort: MatSort | null = null;
  private _filter: string | null = null;
  set filter(value: string | null) {
    this._filter = value;
    this.filterSource.next(value);
  }
  get filter(): string | null {
    return this._filter;
  }
  private filterSource: Subject<string | null> = new Subject<string | null>();
  documentsPaginated: PaginationResult<DocumentGet> | null = null;
  pageSizeOptions = [1, 5, 10, 25, 100];
  displayedColumns = [
    'name',
    'description',
    'fileName',
    'fileSizeInBytes',
    'createdAtUtc',
    'contentAvailable',
  ];

  constructor(
    private route: ActivatedRoute,
    public documentsService: DocumentsService
  ) {}

  ngOnInit(): void {
    this.documentsService.paginationResult
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((r: PaginationResult<DocumentGet>) => {
        this.documentsPaginated = r;
      });
    this.setFilterSettings();
    this.route.params.pipe(takeUntil(this.unsubscribe)).subscribe((params) => {
      if (params.projectId) {
        this.projectId = params.projectId;
        this.documentsService.setProjectId(this.projectId);
      }
    });
  }

  private setFilterSettings(): void {
    this.filter = this.documentsService.getQueryParameter('filter');
    this.documentsService.pageSize = 5;
    this.documentsService.sort = {
      propertyName: 'createdAtUtc',
      isDescending: true,
    };
    this.sort?.sort({
      id: 'createdAtUtc',
      start: 'desc',
      disableClear: true,
    });
    this.filterSource
      .pipe(
        switchMap((x: string | null) => of<string | null>(x).pipe(delay(250))),
        distinctUntilChanged()
      )
      .subscribe((filterValue: string | null) => {
        filterValue
          ? this.documentsService.setQueryParameter('filter', filterValue)
          : this.documentsService.setQueryParameter('filter', '');
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
    this.documentsService.page = 1;
    this.documentsService.setProjectId(null);
  }

  resetFilterSettings(): void {
    this.documentsService.setQueryParameter('filter', '');
  }

  onFilter(filter: string | null): void {
    this.filterSource.next(filter);
  }

  onSort(event: Sort): void {
    this.documentsService.sort = {
      propertyName: event.active,
      isDescending: event.direction === 'desc',
    };
  }

  onPage(pageEvent: PageEvent): void {
    this.documentsService.page = pageEvent.pageIndex + 1;
    this.documentsService.pageSize = pageEvent.pageSize;
  }
}
