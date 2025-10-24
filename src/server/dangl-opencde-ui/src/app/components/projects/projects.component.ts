import { Component, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { MatSort, Sort, MatSortHeader } from '@angular/material/sort';
import { Subject, of } from 'rxjs';
import {
  delay,
  distinctUntilChanged,
  switchMap,
  takeUntil,
} from 'rxjs/operators';

import { PageEvent, MatPaginator } from '@angular/material/paginator';
import { PaginationResult } from 'ng-lightquery';
import { ProjectGet } from '../../generated/backend-client';
import { ProjectsService } from '../../services/projects.service';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
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
import { NgDanglIconsModule } from 'ng-dangl-icons';

@Component({
  selector: 'opencde-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss'],
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
    MatCellDef,
    MatCell,
    NgDanglIconsModule,
    MatSortHeader,
    MatHeaderRowDef,
    MatHeaderRow,
    MatRowDef,
    MatRow,
    MatPaginator,
  ],
})
export class ProjectsComponent implements OnInit, OnDestroy {
  projectsService = inject(ProjectsService);

  @ViewChild(MatSort, { static: true }) private sort: MatSort | null = null;
  private _filter: string | null = null;
  set filter(value: string | null) {
    this._filter = value;
    this.filterSource.next(value);
  }
  get filter(): string | null {
    return this._filter;
  }

  private unsubscribe: Subject<void> = new Subject<void>();
  private filterSource: Subject<string | null> = new Subject<string | null>();
  projectsPaginated: PaginationResult<ProjectGet> | null = null;
  pageSizeOptions = [1, 5, 10, 25, 100];
  displayedColumns = ['identiconId', 'name', 'description'];

  ngOnInit(): void {
    this.projectsService.paginationResult
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((r: PaginationResult<ProjectGet>) => {
        this.projectsPaginated = r;
      });
    this.setFilterSettings();
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
    this.resetFilterSettings();
  }

  private setFilterSettings(): void {
    this.filter = this.projectsService.getQueryParameter('filter');
    this.projectsService.pageSize = 5;
    this.projectsService.sort = {
      propertyName: 'name',
      isDescending: true,
    };
    this.sort?.sort({
      id: 'name',
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
          ? this.projectsService.setQueryParameter('filter', filterValue)
          : this.projectsService.setQueryParameter('filter', '');
      });
  }

  resetFilterSettings(): void {
    this.projectsService.setQueryParameter('filter', '');
  }

  onFilter(filter: string | null): void {
    this.filterSource.next(filter);
  }

  onSort(event: Sort): void {
    this.projectsService.sort = {
      propertyName: event.active,
      isDescending: event.direction === 'desc',
    };
  }

  onPage(pageEvent: PageEvent): void {
    this.projectsService.page = pageEvent.pageIndex + 1;
    this.projectsService.pageSize = pageEvent.pageSize;
  }
}
