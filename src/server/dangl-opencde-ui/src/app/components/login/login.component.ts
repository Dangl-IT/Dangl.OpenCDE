import { Component, OnInit, inject } from '@angular/core';

import { AuthenticationService } from '../../services/authentication.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'opencde-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [NgIf],
})
export class LoginComponent implements OnInit {
  private authenticationService = inject(AuthenticationService);

  processingLoginResponse = true;
  errorMessage: string | null = null;
  error = false;

  ngOnInit(): void {
    this.authenticationService
      .processSignInResponse()
      .subscribe((loginResponse) => {
        this.processingLoginResponse = false;

        if (!loginResponse.success) {
          this.error = true;
          this.errorMessage = loginResponse.error ?? null;
        }
      });
  }
}
