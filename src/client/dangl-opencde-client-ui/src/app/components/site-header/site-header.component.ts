import { Component, OnDestroy, OnInit } from '@angular/core';

import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { HeaderComponent } from '@dangl/angular-material-shared';
import { NgIf } from '@angular/common';
import { UserInfoComponent } from '../user-info/user-info.component';

@Component({
  selector: 'opencde-client-site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.scss'],
  imports: [HeaderComponent, NgIf, UserInfoComponent],
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
