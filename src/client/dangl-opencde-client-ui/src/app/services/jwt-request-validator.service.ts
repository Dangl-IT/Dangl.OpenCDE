import { HttpRequest } from '@angular/common/http';
import { IDanglIdentityRequestValidator } from '@dangl/angular-dangl-identity-client';
import { Injectable } from '@angular/core';
import { OpenCdeDiscoveryService } from './open-cde-discovery.service';

@Injectable({
  providedIn: 'root',
})
export class JwtRequestValidatorService
  implements IDanglIdentityRequestValidator
{
  private openCdeBaseUrl: string | null = null;

  constructor(openCdeDiscoveryService: OpenCdeDiscoveryService) {
    openCdeDiscoveryService.openCdeBaseUrl.subscribe((cdeBaseUrl) => {
      if (!cdeBaseUrl) {
        this.openCdeBaseUrl = null;
        return;
      }

      const url = new URL(cdeBaseUrl);
      this.openCdeBaseUrl = `${url.protocol}//${url.hostname}`.toLowerCase();
    });
  }

  /**
   * This validates if the current request matches the base url of the specified CDE instance. Otherwise,
   * JWT tokens will not be sent. This is important, since we don't want to send JWT tokens to e.g.
   * storage urls when downloading documents.
   * @param httpRequest
   */
  validateRequest(httpRequest: HttpRequest<any>): boolean {
    const requestUrl = httpRequest.url;
    if (!requestUrl || requestUrl.startsWith('/')) {
      // It's fine for local requests
      return true;
    }

    if (!this.openCdeBaseUrl) {
      // Don't have an url yet, so in this example we default to just include the token
      return true;
    }

    return requestUrl.toLowerCase().startsWith(this.openCdeBaseUrl);
  }
}
