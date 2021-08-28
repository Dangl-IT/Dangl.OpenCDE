import { AppConfigService } from './app-config.service';
import { Injectable } from '@angular/core';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { UserManager } from 'oidc-client-ts';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(
    private appConfigService: AppConfigService,
    private jwtTokenService: JwtTokenService,
    private router: Router
  ) {}

  initiateOpenIdImplicitLogin(): void {
    const userManager = this.getUserManager();
    // Here, we're directly creating the sign in process and will redirect the app
    userManager.signinRedirect();
  }

  processSignInResponse(): Observable<{ success: boolean; error?: string }> {
    const userManager = this.getUserManager();
    const loginResult = new Subject<{ success: boolean; error?: string }>();

    userManager.signinRedirectCallback().then(
      (r) => {
        // We're just storing the token from the implicit grant,
        // there's not going to be any refresh token functionality
        this.jwtTokenService.storeCustomToken({
          accessToken: r.access_token,
          expiresAt: r.expires_at,
        });
        this.router.navigateByUrl('/');

        loginResult.next({ success: true });
        loginResult.complete();
      },
      (e) => {
        loginResult.next({ success: false, error: e });
        loginResult.complete();
      }
    );

    return loginResult;
  }

  private getUserManager(): UserManager {
    const appConfig = this.appConfigService.getFrontendConfig();
    return new UserManager({
      authority: appConfig.danglIdentityUrl,
      client_id: appConfig.danglIdentityClientId,
      redirect_uri: `${window.location.origin}/login`,
      scope: `${appConfig.requiredScope} openid`,
      response_type: 'id_token token',
    });
  }
}
