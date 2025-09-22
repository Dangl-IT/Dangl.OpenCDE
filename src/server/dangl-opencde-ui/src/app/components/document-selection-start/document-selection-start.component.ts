import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, inject } from '@angular/core';

import { CdeSessionService } from '../../services/cde-session.service';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenCdeDownloadIntegrationClient } from '../../generated/backend-client';
import { first } from 'rxjs/operators';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'opencde-document-selection-start',
  templateUrl: './document-selection-start.component.html',
  styleUrls: ['./document-selection-start.component.scss'],
  imports: [MatProgressSpinner],
})
export class DocumentSelectionStartComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private openCdeDownloadIntegrationClient = inject(
    OpenCdeDownloadIntegrationClient
  );
  private jwtTokenService = inject(JwtTokenService);
  private cdeSessionService = inject(CdeSessionService);

  ngOnInit(): void {
    this.route.queryParams.pipe(first()).subscribe((p) => {
      if (p.documentSessionId) {
        const documentSessionId: string = p.documentSessionId;
        this.cdeSessionService.setCurrentSessionId(documentSessionId);

        this.openCdeDownloadIntegrationClient
          .getDownloadSessionSimpleAuthData(documentSessionId)
          .subscribe((simpleAuthToken) => {
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
