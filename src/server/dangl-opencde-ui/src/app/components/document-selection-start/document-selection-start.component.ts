import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

import { CdeSessionService } from '../../services/cde-session.service';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenCdeIntegrationClient } from '../../generated/backend-client';
import { first } from 'rxjs/operators';

@Component({
  selector: 'opencde-document-selection-start',
  templateUrl: './document-selection-start.component.html',
  styleUrls: ['./document-selection-start.component.scss'],
})
export class DocumentSelectionStartComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private openCdeIntegrationClient: OpenCdeIntegrationClient,
    private jwtTokenService: JwtTokenService,
    private cdeSessionService: CdeSessionService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(first()).subscribe((p) => {
      if (p.documentSessionId) {
        const documentSessionId: string = p.documentSessionId;
        this.cdeSessionService.setCurrentSessionId(documentSessionId);

        this.openCdeIntegrationClient
          .getSessionSimpleAuthData(documentSessionId)
          .subscribe((simpleAuthToken) => {
            console.log(simpleAuthToken);

            this.jwtTokenService.storeCustomToken({
              accessToken: simpleAuthToken.jwt,
              expiresAt: simpleAuthToken.expiresAt,
            });

            this.router.navigateByUrl('/projects');
          });
      }
    });
  }
}
