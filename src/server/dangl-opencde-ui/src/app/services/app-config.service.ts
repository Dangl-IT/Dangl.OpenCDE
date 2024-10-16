import { Injectable } from '@angular/core';
import { FrontendConfigGet } from '../generated/backend-client';

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  getFrontendConfig(): FrontendConfigGet {
    const frontendConfig = (<any>window).danglOpenCdeFrontendConfig;
    return frontendConfig ? JSON.parse(JSON.stringify(frontendConfig)) : {};
  }
}
