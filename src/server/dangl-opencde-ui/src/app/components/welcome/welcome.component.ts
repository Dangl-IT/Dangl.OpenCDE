import { Component, OnDestroy, OnInit } from '@angular/core';

import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { AuthenticationService } from '../../services/authentication.service';
import { Subject } from 'rxjs';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.scss'],
})
export class WelcomeComponent implements OnInit, OnDestroy {
  userInfo: UserInfo | null = null;

  constructor(
    private authenticationMessenger: AuthenticationMessenger,
    private authenticationService: AuthenticationService
  ) {}
  private unsubscribe: Subject<void> = new Subject<void>();

  ngOnInit(): void {
    this.authenticationMessenger.userInfo
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((ui) => (this.userInfo = ui));
  }

  login(): void {
    this.authenticationService.initiateOpenIdImplicitLogin();
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
}
