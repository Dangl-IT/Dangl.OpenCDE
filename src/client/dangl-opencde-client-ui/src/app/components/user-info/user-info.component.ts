import {
  AuthenticationMessenger,
  AuthenticationService,
} from '@dangl/angular-dangl-identity-client';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { NgIf } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';

@Component({
  selector: 'opencde-client-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
  imports: [NgIf, MatButton, MatMenuTrigger, MatMenu, MatMenuItem, MatIcon],
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
