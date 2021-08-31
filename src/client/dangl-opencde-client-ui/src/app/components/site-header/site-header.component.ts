import { Component, OnDestroy, OnInit } from '@angular/core';

import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { Subject } from 'rxjs';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.scss'],
})
export class SiteHeaderComponent implements OnInit, OnDestroy {
  showPreReleaseHeader = false;
  menuOpened = false;
  userInfo: UserInfo | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(private authenticationMessenger: AuthenticationMessenger) {}

  ngOnInit(): void {
    this.authenticationMessenger.userInfo
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((ui) => (this.userInfo = ui));
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
}
