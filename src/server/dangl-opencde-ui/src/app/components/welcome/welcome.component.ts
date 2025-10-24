import { Component, OnDestroy, OnInit, inject } from '@angular/core';

import { AuthenticationMessenger } from '@dangl/angular-dangl-identity-client';
import { AuthenticationService } from '../../services/authentication.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import {
  MatCard,
  MatCardHeader,
  MatCardContent,
  MatCardActions,
} from '@angular/material/card';
import { MatAnchor } from '@angular/material/button';
import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';

@Component({
  selector: 'opencde-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.scss'],
  imports: [MatCard, MatCardHeader, MatCardContent, MatCardActions, MatAnchor],
})
export class WelcomeComponent implements OnInit, OnDestroy {
  private authenticationMessenger = inject(AuthenticationMessenger);
  private authenticationService = inject(AuthenticationService);

  userInfo: UserInfo | null = null;
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
