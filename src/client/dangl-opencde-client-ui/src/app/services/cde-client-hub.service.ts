import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import { DocumentSelectionService } from './document-selection.service';
import { Injectable } from '@angular/core';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenIdConnectAuthenticationResult } from '../generated/backend-client';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CdeClientHubService {
  private connection: HubConnection | null = null;
  isAuthenticaed = false;
  private lastClientState: string | null = null;
  private authenticationResultReceivedSource = new Subject<{
    state: string;
    isSuccess: boolean;
  }>();
  authenticationResultReceived =
    this.authenticationResultReceivedSource.asObservable();

  constructor(
    private jwtTokenService: JwtTokenService,
    private documentSelectionService: DocumentSelectionService
  ) {
    this.buildConnection();
    this.setUpMessageListeners();
    this.connection?.start();
  }

  private buildConnection(): void {
    this.connection = new HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl('/hubs/cde-client')
      .build();
  }

  private setUpMessageListeners(): void {
    if (!this.connection) {
      return;
    }

    this.connection.on(
      'OpenIdConnectCallback',
      (response: OpenIdConnectAuthenticationResult) => {
        this.authenticationResultReceivedSource.next({
          state: response.clientState,
          isSuccess: response.isSuccess,
        });

        if (response.clientState !== this.lastClientState) {
          console.warn(
            'Received an OAuth2 response for a no-longer valid state: ' +
              response.clientState
          );
          return;
        }

        if (response.isSuccess) {
          try {
            this.jwtTokenService.storeCustomToken({
              accessToken: response.jwtToken!,
              expiresAt: response.expiresAt!,
            });
          } catch {
            /* Some services might not return Jwt tokens, but that's fine😀 */
          }
        } else {
          alert('Authentication failed');
        }
      }
    );

    this.connection.on(
      'DocumentSelectionResultCallback',
      (response: { selectedDocumentsUrl: string; state: string }) => {
        this.documentSelectionService.setSelectedDocument(
          response.selectedDocumentsUrl
        );
      }
    );
  }

  setOpenIdAuthClientState(clientState: string) {
    this.lastClientState = clientState;
  }
}
