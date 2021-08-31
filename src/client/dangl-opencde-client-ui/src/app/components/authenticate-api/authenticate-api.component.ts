import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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
import { filter, first } from 'rxjs/operators';

import { AuthGet } from '../../generated/opencde-client';
import { CdeClientHubService } from '../../services/cde-client-hub.service';
import { GuidGenerator } from '@dangl/angular-material-shared';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';

@Component({
  selector: 'opencde-client-authenticate-api',
  templateUrl: './authenticate-api.component.html',
  styleUrls: ['./authenticate-api.component.scss'],
})
export class AuthenticateApiComponent implements OnInit {
  openIdForm: FormGroup;
  authenticationInformation: AuthGet | null = null;
  authenticationInProgress = false;
  lastAuthenticationFailed = false;
  @Output() onAuthentication = new EventEmitter<void>();

  constructor(
    private openCdeDiscoveryService: OpenCdeDiscoveryService,
    formBuilder: FormBuilder,
    private cdeClientHubService: CdeClientHubService,
    private openIdClient: OpenIdClient
  ) {
    this.openIdForm = formBuilder.group({
      clientId: new FormControl(
        'a4e59a6a-a300-48e3-89a0-2cfbde8ce304',
        Validators.required
      ),
      clientSecret: new FormControl(''),
      flow: new FormControl('', Validators.required),
      requiredScope: new FormControl(''),
    });
  }

  ngOnInit(): void {
    this.openCdeDiscoveryService.foundationsAuthentication
      .pipe(first())
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

  authenticate(): void {
    if (!this.authenticationInformation) {
      return;
    }

    const clientState = GuidGenerator.generatePseudoRandomGuid();

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
    const authenticationParameters: OpenIdConnectAuthenticationParameters = {
      clientState: clientState,
      flow: openIdFlow,
      clientConfiguration: {
        authorizeEndpoint: this.authenticationInformation.oauth2_auth_url!, // We're checking earlier to ensure this is present
        clientId: this.openIdForm.value.clientId,
        clientSecret: this.openIdForm.value.clientSecret,
        requiredScope: this.openIdForm.value.requiredScope,
        tokenEndpoint: this.authenticationInformation.oauth2_token_url,
      },
    };

    this.cdeClientHubService.setOpenIdAuthClientState(clientState);
    this.openIdClient
      .initiateOpenIdConnectImplicitLogin(authenticationParameters)
      .subscribe(() => {});
  }
}
