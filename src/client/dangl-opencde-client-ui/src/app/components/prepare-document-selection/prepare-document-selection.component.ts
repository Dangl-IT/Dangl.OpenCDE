import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  DocumentDiscoveryPost,
  DocumentDiscoverySessionInitialization,
  DocumentSelectionClient,
} from '../../generated/opencde-client';
import { first, map } from 'rxjs/operators';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { DocumentsSelectionHandlerClient } from '../../generated/backend-client';
import { GuidGenerator } from '@dangl/angular-material-shared';
import { HttpClient } from '@angular/common/http';
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
    private http: HttpClient,
    private documentsSelectionHandlerClient: DocumentsSelectionHandlerClient,
    private documentSelectionService: DocumentSelectionService,
    private openCdeDiscoveryService: OpenCdeDiscoveryService
  ) {}

  ngOnInit(): void {}

  initiateDocumentSelection(): void {
    const clientState = GuidGenerator.generatePseudoRandomGuid();

    combineLatest([
      this.openCdeDiscoveryService.openCdeBaseUrl,
      this.documentsSelectionHandlerClient.getDocumentSelectionCallbackUrl(
        clientState
      ),
    ])
      .pipe(
        map((result) => {
          return {
            baseUrl: result[0],
            callbackUrl: result[1].callbackUrl,
          };
        })
      )
      .subscribe((values) => {
        const documentsBaseUrl = `${values.baseUrl}/documents/1.0/select-documents`;

        const documentDiscoveryPost: DocumentDiscoveryPost = {
          callback: {
            expires_in: 3600,
            url: values.callbackUrl,
          },
          state: clientState,
        };
        this.http
          .post<DocumentDiscoverySessionInitialization>(
            documentsBaseUrl,
            documentDiscoveryPost
          )
          .subscribe((response) => {
            this.documentsSelectionHandlerClient
              .openCdeDocumentSelectionPage(response.selectDocumentsUrl)
              .subscribe(() => {
                this.documentSelectionService.referenceLink
                  .pipe(first())
                  .subscribe(() => this.onDocumentSelected.next());
              });
          });
      });
  }
}
