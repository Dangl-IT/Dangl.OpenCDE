import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSort, Sort } from '@angular/material/sort';
import {
  OpenCdeUploadIntegrationClient,
  ProjectGet,
} from '../../generated/backend-client';
import { Subject, of } from 'rxjs';
import {
  delay,
  distinctUntilChanged,
  first,
  switchMap,
  takeUntil,
} from 'rxjs/operators';

import { ActivatedRoute } from '@angular/router';
import { CdeSessionService } from '../../services/cde-session.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { PageEvent } from '@angular/material/paginator';
import { PaginationResult } from 'ng-lightquery';
import { ProjectsService } from '../../services/projects.service';

@Component({
  selector: 'opencde-cde-file-upload',
  templateUrl: './cde-file-upload.component.html',
  styleUrls: ['./cde-file-upload.component.scss'],
})
export class CdeFileUploadComponent implements OnInit, OnDestroy {
  @ViewChild(MatSort, { static: true }) private sort: MatSort | null = null;
  private _filter: string | null = null;
  set filter(value: string | null) {
    this._filter = value;
    this.filterSource.next(value);
  }
  get filter(): string | null {
    return this._filter;
  }
  projectsPaginated: PaginationResult<ProjectGet> | null = null;
  pageSizeOptions = [1, 5, 10, 25, 100];
  displayedColumns = ['identiconId', 'name', 'description'];
  private unsubscribe: Subject<void> = new Subject<void>();
  private filterSource: Subject<string | null> = new Subject<string | null>();

  constructor(
    public projectsService: ProjectsService,
    private route: ActivatedRoute,
    private openCdeUploadIntegrationClient: OpenCdeUploadIntegrationClient,
    private cdeSessionService: CdeSessionService,
    private jwtTokenService: JwtTokenService
  ) {}

  ngOnInit(): void {
    this.route.queryParams
      .pipe(first(), takeUntil(this.unsubscribe))
      .subscribe((p) => {
        let documentSessionId: string | null = null;
        if (p.documentSessionId) {
          documentSessionId = p.documentSessionId;
          this.cdeSessionService.setCurrentSessionId(documentSessionId!);
        }

        if (p.access_token) {
          const accessTokenFromQuery = p.access_token;
          this.setAccessTokenAndReloadProjects(accessTokenFromQuery);
        } else if (documentSessionId) {
          this.openCdeUploadIntegrationClient
            .getUploadSessionSimpleAuthData(documentSessionId)
            .subscribe((simpleAuth) => {
              this.setAccessTokenAndReloadProjects(simpleAuth.jwt);
            });
        }
      });

    this.projectsService.paginationResult
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((r: PaginationResult<ProjectGet>) => {
        this.projectsPaginated = r;
      });
    this.setFilterSettings();
  }

  private setAccessTokenAndReloadProjects(accessToken: string) {
    setTimeout(() => {
      const decodedToken = new JwtHelperService().decodeToken(accessToken);
      this.jwtTokenService.storeCustomToken({
        accessToken: accessToken,
        expiresAt: decodedToken.exp,
      });

      this.projectsService.forceRefresh();
    }, 0);
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

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
    this.resetFilterSettings();
  }

  selectPoject(project: ProjectGet): void {
    this.cdeSessionService.sessionId
      .pipe(first(), takeUntil(this.unsubscribe))
      .subscribe((sessionId) => {
        this.openCdeUploadIntegrationClient
          .setProjectForUploadSession(sessionId, {
            projectId: project.id,
          })
          .subscribe((r) => {
            window.location.href = r.clientCallbackUrl;
          });
      });
  }

  resetFilterSettings(): void {
    this.projectsService.setQueryParameter('filter', '');
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
