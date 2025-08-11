import { Component, Input } from '@angular/core';

import { AuthenticationService } from '@dangl/angular-dangl-identity-client';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';
import { NgIf } from '@angular/common';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { MatButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';

@Component({
  selector: 'opencde-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
  imports: [
    NgIf,
    NgDanglIconsModule,
    MatButton,
    MatMenuTrigger,
    MatMenu,
    MatMenuItem,
    MatIcon,
  ],
})
export class UserInfoComponent {
  @Input() userInfo: UserInfo | null = null;

  constructor(private authenticationService: AuthenticationService) {}

  logout(): void {
    this.authenticationService.logout();
  }
}
