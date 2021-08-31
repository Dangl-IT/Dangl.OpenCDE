import {
  AuthGet,
  VersionsClient,
  VersionsGet,
} from '../generated/opencde-client';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OpenCdeDiscoveryService {
  private foundationsVersionsSource = new ReplaySubject<VersionsGet[]>(1);
  foundationsVersions = this.foundationsVersionsSource.asObservable();

  private foundationsBaseUrlSource = new ReplaySubject<string>(1);
  foundationsBaseUrl = this.foundationsBaseUrlSource.asObservable();

  private openCdeBaseUrlSource = new ReplaySubject<string>(1);
  openCdeBaseUrl = this.openCdeBaseUrlSource.asObservable();

  private foundationsAuthenticationInfoSource = new ReplaySubject<AuthGet>(1);
  foundationsAuthentication =
    this.foundationsAuthenticationInfoSource.asObservable();

  constructor(private http: HttpClient) {
    this.foundationsBaseUrl.subscribe((baseUrl) => {
      if (baseUrl) {
        baseUrl = baseUrl.replace(/\/$/, ''); // Removing trailing slash
        const authUrl = `${baseUrl}/1.0/auth`;
        this.http.get<AuthGet>(authUrl).subscribe((authenticationResponse) => {
          if (!authenticationResponse.oauth2_auth_url) {
            alert("Missing 'oauth2_auth_url' in response");
          } else {
            this.foundationsAuthenticationInfoSource.next(
              authenticationResponse
            );
          }
        });
      }
    });
  }

  setOpenCdeServerBaseUrl(serverBaseUrl: string): void {
    serverBaseUrl = serverBaseUrl.replace(/\/$/, ''); // Removing trailing slash
    const versionsClient = new VersionsClient(this.http, serverBaseUrl);

    versionsClient.getApiVersions().subscribe(
      (response) => {
        this.foundationsVersionsSource.next(response);

        const foundationInfo = response.find(
          (version) => version.api_id === 'foundation'
        );
        if (foundationInfo) {
          this.foundationsBaseUrlSource.next(foundationInfo.api_base_url);
        }

        const openCdeInfo = response.find(
          (version) => version.api_id === 'opencde'
        );
        if (openCdeInfo) {
          this.openCdeBaseUrlSource.next(openCdeInfo.api_base_url);
        } else {
          alert("Failed to find 'opencde' version on server");
        }
      },
      () => {
        alert('Failed to get Foundations API versions');
      }
    );
  }
}
