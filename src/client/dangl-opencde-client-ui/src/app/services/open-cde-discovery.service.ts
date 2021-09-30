import {
  AuthGet,
  VersionGet,
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
  private foundationsVersionsSource = new ReplaySubject<VersionGet[]>(1);
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
        let authUrl = `${baseUrl}/auth`;

        const getProxyUrl = (actualUrl: string) =>
          `/client-proxy?&targetUrl=${encodeURIComponent(actualUrl)}`;

        this.http.get<AuthGet>(getProxyUrl(authUrl)).subscribe(
          (authenticationResponse) => {
            if (!authenticationResponse.oauth2_auth_url) {
              alert("Missing 'oauth2_auth_url' in response");
            } else {
              this.foundationsAuthenticationInfoSource.next(
                authenticationResponse
              );
            }
          },
          () => {
            authUrl = `${baseUrl}/1.0/auth`;
            this.http
              .get<AuthGet>(getProxyUrl(authUrl))
              .subscribe((authenticationResponse) => {
                if (!authenticationResponse.oauth2_auth_url) {
                  alert("Missing 'oauth2_auth_url' in response");
                } else {
                  this.foundationsAuthenticationInfoSource.next(
                    authenticationResponse
                  );
                }
              });
          }
        );
      }
    });
  }

  setOpenCdeServerBaseUrl(serverBaseUrl: string): void {
    serverBaseUrl = serverBaseUrl.replace(/\/$/, ''); // Removing trailing slash

    const handleVersionsResponse = (response: VersionsGet) => {
      const versions = response.versions;
      if (!versions) {
        return;
      }

      this.foundationsVersionsSource.next(versions);

      const foundationInfo = versions.find(
        (version) => version.api_id === 'foundation'
      );
      if (foundationInfo) {
        if (foundationInfo.api_base_url) {
          this.foundationsBaseUrlSource.next(foundationInfo.api_base_url);
        } else {
          // Some servers might not return the 'api_base_url' directly for
          // the Foundation API, so we assume that they follow the convention
          // of having it just relative at '/foundation' to the base url
          const foundationsBaseUrl = `${serverBaseUrl}/foundation`;
          this.foundationsBaseUrlSource.next(foundationsBaseUrl);
        }
      }

      const openCdeInfo = versions.find(
        (version) =>
          version.api_id === 'opencde' || version.api_id === 'documents'
      );
      if (openCdeInfo) {
        this.openCdeBaseUrlSource.next(openCdeInfo.api_base_url);
      } else {
        alert("Failed to find 'opencde' or 'documents' version on server");
      }
    };

    if (serverBaseUrl.toLowerCase().indexOf('trimble') > -1) {
      // There's a special workaround here at the moment, since the foundations versions
      // endpoint is not available yet
      const trimbleResponse: VersionsGet = {
        versions: [
          {
            api_id: 'foundation',
            version_id: '1.0',
            detailed_version:
              'https://github.com/BuildingSMART/foundation-API/tree/release_1_0',
            api_base_url: 'https://auth.connect.trimble.com/v1/foundation/1.0',
          },
          {
            api_id: 'documents',
            version_id: '1.0',
            detailed_version:
              'https://github.com/BuildingSMART/foundation-API/tree/release_1_0',
            api_base_url: 'https://auth.connect.trimble.com/v1/foundation/1.0',
          },
        ],
      };

      handleVersionsResponse(trimbleResponse);
    } else {
      const getProxyUrl = (actualUrl: string) =>
        `/client-proxy?targetUrl=${encodeURIComponent(actualUrl)}`;

      const versionsBaseUrl = getProxyUrl(
        `${serverBaseUrl}/foundation/versions`
      );
      this.http.get<VersionsGet>(versionsBaseUrl).subscribe(
        (response) => {
          handleVersionsResponse(response);
        },
        () => {
          alert('Failed to get Foundations API versions');
        }
      );
    }
  }
}
