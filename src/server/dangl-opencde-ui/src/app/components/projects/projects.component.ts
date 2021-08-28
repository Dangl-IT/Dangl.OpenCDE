import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSort, Sort } from '@angular/material/sort';
import { Subject, of } from 'rxjs';
import {
  delay,
  distinctUntilChanged,
  switchMap,
  takeUntil,
} from 'rxjs/operators';

import { PageEvent } from '@angular/material/paginator';
import { PaginationResult } from 'ng-lightquery';
import { ProjectGet } from '../../generated/backend-client';
import { ProjectsService } from '../../services/projects.service';

@Component({
  selector: 'opencde-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss'],
})
export class ProjectsComponent implements OnInit, OnDestroy {
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

  constructor(public projectsService: ProjectsService) {}

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
