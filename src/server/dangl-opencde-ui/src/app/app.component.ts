import { Component, OnDestroy, OnInit } from '@angular/core';

import { AppConfigService } from './services/app-config.service';
import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { DanglIconsConfigService } from 'ng-dangl-icons';
import { IconRegistry } from './icon-registry';
import { SidebarService } from './services/sidebar.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  sideNavOpened = false;
  userIsAuthenticated = false;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    appConfigService: AppConfigService,
    danglIconsConfigService: DanglIconsConfigService,
    private sidebarService: SidebarService,
    private authenticationMessenger: AuthenticationMessenger,
    iconRegistry: IconRegistry
  ) {
    danglIconsConfigService.setConfig({
      baseUrl: appConfigService.getFrontendConfig().danglIconsBaseUrl,
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
