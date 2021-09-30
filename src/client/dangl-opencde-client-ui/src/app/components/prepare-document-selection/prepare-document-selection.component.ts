import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { first, map } from 'rxjs/operators';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { DocumentsSelectionHandlerClient } from '../../generated/backend-client';
import { GuidGenerator } from '@dangl/angular-material-shared';
import { HttpClient } from '@angular/common/http';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'opencde-client-prepare-document-selection',
  templateUrl: './prepare-document-selection.component.html',
  styleUrls: ['./prepare-document-selection.component.scss'],
})
export class PrepareDocumentSelectionComponent implements OnInit {
  @Output() onDocumentSelected = new EventEmitter<void>();

  constructor(
    private documentsSelectionHandlerClient: DocumentsSelectionHandlerClient,
    private documentSelectionService: DocumentSelectionService,
    private openCdeDiscoveryService: OpenCdeDiscoveryService,
    private jwtTokenService: JwtTokenService
  ) {}

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
