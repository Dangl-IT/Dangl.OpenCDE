import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { first, map } from 'rxjs/operators';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { DocumentsSelectionHandlerClient } from '../../generated/backend-client';
import { GuidGenerator } from '@dangl/angular-material-shared/guid-generator';
import { HttpClient } from '@angular/common/http';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { combineLatest } from 'rxjs';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'opencde-client-prepare-document-selection',
  templateUrl: './prepare-document-selection.component.html',
  styleUrls: ['./prepare-document-selection.component.scss'],
  imports: [MatButton],
})
export class PrepareDocumentSelectionComponent implements OnInit {
  private documentsSelectionHandlerClient = inject(
    DocumentsSelectionHandlerClient
  );
  private documentSelectionService = inject(DocumentSelectionService);
  private openCdeDiscoveryService = inject(OpenCdeDiscoveryService);
  private jwtTokenService = inject(JwtTokenService);

  @Output() onDocumentSelected = new EventEmitter<void>();

  ngOnInit(): void {}

  initiateDocumentSelection(): void {
    const clientState = GuidGenerator.generatePseudoRandomGuid();

    combineLatest([
      this.openCdeDiscoveryService.openCdeBaseUrl,
      this.jwtTokenService.getToken(),
    ])
      .pipe(
        first(),
        map((result) => {
          return {
            baseUrl: result[0],
            token: result[1].accessToken,
          };
        })
      )
      .subscribe((values) => {
        this.documentsSelectionHandlerClient
          .prepareDocumentSelectionAndOpenSystemBrowser({
            accessToken: values.token,
            clientState: clientState,
            openCdeBaseUrl: values?.baseUrl ?? '',
          })
          .subscribe(() => {
            this.documentSelectionService.referenceLink
              .pipe(first())
              .subscribe(() => this.onDocumentSelected.next());
          });
      });
  }
}
