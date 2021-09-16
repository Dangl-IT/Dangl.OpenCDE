import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  OpenIdClient,
  OpenIdConnectAuthenticationParameters,
  OpenIdConnectFlowType,
} from '../../generated/backend-client';
import { filter, first, takeUntil } from 'rxjs/operators';

import { AuthGet } from '../../generated/opencde-client';
import { CdeClientHubService } from '../../services/cde-client-hub.service';
import { GuidGenerator } from '@dangl/angular-material-shared';
import { ManageOpenidConfigsModalComponent } from '../manage-openid-configs-modal/manage-openid-configs-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { SettingsService } from '../../services/settings.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'opencde-client-authenticate-api',
  templateUrl: './authenticate-api.component.html',
  styleUrls: ['./authenticate-api.component.scss'],
})
export class AuthenticateApiComponent implements OnInit, OnDestroy {
  openIdForm: FormGroup;
  authenticationInformation: AuthGet | null = null;
  authenticationInProgress = false;
  lastAuthenticationFailed = false;
  cdeServerBaseUrl: string | null = null;
  @Output() onAuthentication = new EventEmitter<void>();
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private openCdeDiscoveryService: OpenCdeDiscoveryService,
    formBuilder: FormBuilder,
    private cdeClientHubService: CdeClientHubService,
    private openIdClient: OpenIdClient,
    private settingsService: SettingsService,
    private matDialog: MatDialog
  ) {
    this.openIdForm = formBuilder.group({
      clientId: new FormControl('', Validators.required),
      clientSecret: new FormControl(''),
      flow: new FormControl('', Validators.required),
      requiredScope: new FormControl(''),
      customRedirectUrl: new FormControl(''),
    });
  }

  ngOnInit(): void {
    this.openCdeDiscoveryService.foundationsBaseUrl
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((baseUrl) => {
        let cdeServerBaseUrl = baseUrl.replace(/\/$/, '').toLowerCase();
        if (cdeServerBaseUrl.endsWith('foundation')) {
          cdeServerBaseUrl = cdeServerBaseUrl.substring(
            0,
            cdeServerBaseUrl.length - 'foundation'.length
          );
        }
        this.cdeServerBaseUrl = cdeServerBaseUrl;

        const settings = this.settingsService.getSettings();
        if (settings.clientConfigurations[this.cdeServerBaseUrl]) {
          this.applyLoadedConfig(
            settings.clientConfigurations[this.cdeServerBaseUrl]
          );
        }
      });

    this.openCdeDiscoveryService.foundationsAuthentication
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((authenticationInformation) => {
        this.authenticationInformation = authenticationInformation;

        if (this.authenticationInformation.oauth2_required_scopes) {
          this.openIdForm.patchValue({
            requiredScope:
              this.authenticationInformation.oauth2_required_scopes,
          });
        }

        if (
          this.authenticationInformation.supported_oauth2_flows &&
          this.authenticationInformation.supported_oauth2_flows.length > 0
        ) {
          if (
            this.authenticationInformation.supported_oauth2_flows.findIndex(
              (f) => f === 'implicit_grant'
            ) > -1
          ) {
            this.openIdForm.patchValue({
              flow: 'implicit_grant',
            });
          } else {
            this.openIdForm.patchValue({
              flow: this.authenticationInformation.supported_oauth2_flows[0],
            });
          }
        }
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  loadConfig(): void {
    this.matDialog
      .open(ManageOpenidConfigsModalComponent)
      .afterClosed()
      .subscribe(
        (
          authenticationParameters: OpenIdConnectAuthenticationParameters | null
        ) => {
          if (authenticationParameters) {
            this.applyLoadedConfig(authenticationParameters);
          }
        }
      );
  }

  private applyLoadedConfig(
    authenticationParameters: OpenIdConnectAuthenticationParameters
  ): void {
    this.openIdForm.patchValue({
      clientId: authenticationParameters.clientConfiguration.clientId,
      clientSecret: authenticationParameters.clientConfiguration.clientSecret,
      flow: authenticationParameters.flow,
      requiredScope: authenticationParameters.clientConfiguration.requiredScope,
      customRedirectUrl:
        authenticationParameters.clientConfiguration.customRedirectUrl,
    });
  }

  saveSettings(): void {
    const authenticationParameters = this.getAuthenticationParameters();
    if (authenticationParameters && this.cdeServerBaseUrl) {
      switch (authenticationParameters.flow) {
        case OpenIdConnectFlowType.Implicit:
          authenticationParameters.flow = <any>'implicit_grant';
          break;
        case OpenIdConnectFlowType.AuthorizationCode:
          authenticationParameters.flow = <any>'authorization_code_grant';
          break;
      }

      this.settingsService.saveClientConfiguration(
        this.cdeServerBaseUrl,
        authenticationParameters
      );
    }
  }

  authenticate(): void {
    if (!this.authenticationInformation) {
      return;
    }

    const clientState = GuidGenerator.generatePseudoRandomGuid();

    this.cdeClientHubService.authenticationResultReceived
      .pipe(
        filter((r) => r.state === clientState),
        first()
      )
      .subscribe((r) => {
        this.authenticationInProgress = false;
        this.lastAuthenticationFailed = !r.isSuccess;
        if (r.isSuccess) {
          this.onAuthentication.next();
        }
      });

    this.authenticationInProgress = true;

    const authenticationParameters = this.getAuthenticationParameters();
    if (!authenticationParameters) {
      return;
    }

    authenticationParameters.clientState = clientState;

    this.cdeClientHubService.setOpenIdAuthClientState(clientState);
    this.openIdClient
      .initiateOpenIdConnectImplicitLogin(authenticationParameters)
      .subscribe(() => {});
  }

  private getAuthenticationParameters():
    | OpenIdConnectAuthenticationParameters
    | undefined {
    if (!this.authenticationInformation) {
      return;
    }

    let openIdFlow = OpenIdConnectFlowType.Implicit;
    switch (this.openIdForm.value.flow) {
      case 'implicit_grant':
        openIdFlow = OpenIdConnectFlowType.Implicit;
        break;
      case 'authorization_code_grant':
        openIdFlow = OpenIdConnectFlowType.AuthorizationCode;
        break;
      default:
        alert('Unsupported OAuth2 flow type in this example');
        return;
    }

    return {
      clientState: '',
      flow: openIdFlow,
      clientConfiguration: {
        authorizeEndpoint: this.authenticationInformation.oauth2_auth_url!, // We're checking earlier to ensure this is present
        clientId: this.openIdForm.value.clientId,
        clientSecret: this.openIdForm.value.clientSecret,
        requiredScope: this.openIdForm.value.requiredScope,
        tokenEndpoint: this.authenticationInformation.oauth2_token_url,
        customRedirectUrl: this.openIdForm.value.customRedirectUrl,
      },
    };
  }
}
