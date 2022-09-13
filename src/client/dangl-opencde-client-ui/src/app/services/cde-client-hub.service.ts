import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ReplaySubject, Subject } from 'rxjs';

import { DocumentSelectionService } from './document-selection.service';
import { DocumentVersion } from '../generated/open-cde-swagger/model/documentVersion';
import { Injectable } from '@angular/core';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { NotificationService } from './notification.service';
import { OpenIdConnectAuthenticationResult } from '../generated/backend-client';

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

  private documentVersionUploadResultReceivedSource =
    new ReplaySubject<DocumentVersion>();
  documentVersionUploadResultReceived =
    this.documentVersionUploadResultReceivedSource.asObservable();

  constructor(
    private jwtTokenService: JwtTokenService,
    private documentSelectionService: DocumentSelectionService,
    private notificationService: NotificationService
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

    this.connection.on(
      'DocumentVersionUploaded',
      (documentVersion: DocumentVersion) => {
        this.documentVersionUploadResultReceivedSource.next(documentVersion);
      }
    );

    this.connection.on(
      'NotificationMessage',
      (messageData: { isError: boolean; message: string }) => {
        if (messageData.isError) {
          this.notificationService.showErrorMessage(messageData.message);
        } else {
          this.notificationService.showInfoMessage(messageData.message);
        }
      }
    );
  }

  setOpenIdAuthClientState(clientState: string) {
    this.lastClientState = clientState;
  }
}
