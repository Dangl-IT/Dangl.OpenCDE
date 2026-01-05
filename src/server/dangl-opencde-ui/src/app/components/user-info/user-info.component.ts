import { Component, Input, inject } from '@angular/core';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';

import { AuthenticationService } from '@dangl/angular-dangl-identity-client';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { NgDanglIconsModule } from 'ng-dangl-icons';

import { UserInfo } from 'node_modules/@dangl/angular-dangl-identity-client/models/user-info';

@Component({
  selector: 'opencde-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
  standalone: true,
  imports: [
    NgDanglIconsModule,
    MatButton,
    MatMenuTrigger,
    MatMenu,
    MatMenuItem,
    MatIcon,
  ],
})
export class UserInfoComponent {
  private authenticationService = inject(AuthenticationService);

  @Input() userInfo: UserInfo | null = null;

  logout(): void {
    this.authenticationService.logout();
  }
}
