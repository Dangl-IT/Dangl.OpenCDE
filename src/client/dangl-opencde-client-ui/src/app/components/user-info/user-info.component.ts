import {
  AuthenticationMessenger,
  AuthenticationService,
} from '@dangl/angular-dangl-identity-client';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { Subject } from 'rxjs';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
})
export class UserInfoComponent implements OnInit, OnDestroy {
  @Input() userInfo: UserInfo | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private authenticationService: AuthenticationService,
    private authenticationMessenger: AuthenticationMessenger
  ) {}

  logout(): void {
    this.authenticationService.logout();
  }

  ngOnInit(): void {
    this.authenticationMessenger.userInfo
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((ui) => {
        this.userInfo = ui;
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
}
