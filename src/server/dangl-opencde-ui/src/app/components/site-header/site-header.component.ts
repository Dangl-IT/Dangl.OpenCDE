import { Component, OnDestroy, OnInit, inject } from '@angular/core';

import { AppConfigService } from '../../services/app-config.service';
import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { AuthenticationService } from '../../services/authentication.service';
import { HeaderComponent } from '@dangl/angular-material-shared';
import { MatButton } from '@angular/material/button';
import { SidebarService } from '../../services/sidebar.service';
import { Subject } from 'rxjs';
import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';
import { UserInfoComponent } from '../user-info/user-info.component';
import { takeUntil } from 'rxjs/operators';
import { version } from '../../version';

@Component({
  selector: 'opencde-site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.scss'],
  imports: [HeaderComponent, UserInfoComponent, MatButton],
})
export class SiteHeaderComponent implements OnInit, OnDestroy {
  private sidebarService = inject(SidebarService);
  private authenticationService = inject(AuthenticationService);
  private authenticationMessenger = inject(AuthenticationMessenger);

  preReleaseVersion: string = '';
  preReleaseBuildDate: Date;
  showPreReleaseHeader = false;
  menuOpened = false;
  userInfo: UserInfo | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor() {
    const appConfigService = inject(AppConfigService);

    if (
      appConfigService &&
      appConfigService.getFrontendConfig()?.environment !== 'Production'
    ) {
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
