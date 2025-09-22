import { Component, OnDestroy, OnInit, inject } from '@angular/core';

import { AppConfigService } from './services/app-config.service';
import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { DanglIconsConfigService } from 'ng-dangl-icons';
import { IconRegistry } from './icon-registry';
import { SidebarService } from './services/sidebar.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SiteHeaderComponent } from './components/site-header/site-header.component';
import {
  MatSidenavContainer,
  MatSidenav,
  MatSidenavContent,
} from '@angular/material/sidenav';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { RouterOutlet } from '@angular/router';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';

@Component({
  selector: 'opencde-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    SiteHeaderComponent,
    MatSidenavContainer,
    MatSidenav,
    SidenavComponent,
    MatSidenavContent,
    RouterOutlet,
    SiteFooterComponent,
  ],
})
export class AppComponent implements OnInit, OnDestroy {
  private sidebarService = inject(SidebarService);
  private authenticationMessenger = inject(AuthenticationMessenger);

  sideNavOpened = false;
  userIsAuthenticated = false;
  title = 'dangl-opencde-ui';
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor() {
    const appConfigService = inject(AppConfigService);
    const danglIconsConfigService = inject(DanglIconsConfigService);
    const iconRegistry = inject(IconRegistry);

    danglIconsConfigService.setConfig({
      baseUrl: appConfigService.getFrontendConfig()?.danglIconsBaseUrl || '',
    });

    iconRegistry.registerSvgIcons();
  }

  ngOnInit(): void {
    this.sidebarService.isOpen
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((isOpened) => {
        this.sideNavOpened = isOpened;
      });

    this.authenticationMessenger.isAuthenticated
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
        (isAuthenticated) => (this.userIsAuthenticated = isAuthenticated)
      );
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
}
