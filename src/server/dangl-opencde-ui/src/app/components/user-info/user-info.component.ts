import { Component, Input } from '@angular/core';

import { AuthenticationService } from '@dangl/angular-dangl-identity-client';
import { UserInfo } from '@dangl/angular-dangl-identity-client/models/user-info';

@Component({
  selector: 'opencde-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
})
export class UserInfoComponent {
  @Input() userInfo: UserInfo | null = null;

  constructor(private authenticationService: AuthenticationService) {}

  logout(): void {
    this.authenticationService.logout();
  }
}
