import { Component, OnDestroy, OnInit } from '@angular/core';

import { AppConfigService } from '../../services/app-config.service';
import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { AuthenticationService } from '../../services/authentication.service';
import { SidebarService } from '../../services/sidebar.service';
import { Subject } from 'rxjs';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';
import { takeUntil } from 'rxjs/operators';
import { version } from '../../version';

@Component({
  selector: 'opencde-site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.scss'],
})
export class SiteHeaderComponent implements OnInit, OnDestroy {
  preReleaseVersion: string = '';
  preReleaseBuildDate: Date;
  showPreReleaseHeader = false;
  menuOpened = false;
  userInfo: UserInfo | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private sidebarService: SidebarService,
    private authenticationService: AuthenticationService,
    private authenticationMessenger: AuthenticationMessenger,
    appConfigService: AppConfigService
  ) {
    if (appConfigService.getFrontendConfig().environment !== 'Production') {
      this.showPreReleaseHeader = true;
    }
    this.preReleaseVersion = version.version;
    this.preReleaseBuildDate = version.buildDateUtc;
  }

  ngOnInit(): void {
    this.authenticationMessenger.userInfo
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((ui) => (this.userInfo = ui));
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  toggleMenuButton(): void {
    this.menuOpened = !this.menuOpened;
    this.sidebarService.setSideNavStatus(this.menuOpened);
  }

  login(): void {
    this.authenticationService.initiateOpenIdImplicitLogin();
  }
}
