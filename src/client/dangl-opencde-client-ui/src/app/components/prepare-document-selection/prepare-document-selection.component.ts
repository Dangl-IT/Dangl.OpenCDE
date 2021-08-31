import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { DocumentSelectionClient } from '../../generated/opencde-client';
import { DocumentSelectionService } from '../../services/document-selection.service';
import { DocumentsSelectionHandlerClient } from '../../generated/backend-client';
import { GuidGenerator } from '@dangl/angular-material-shared';
import { HttpClient } from '@angular/common/http';
import { first } from 'rxjs/operators';

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
    private documentSelectionService: DocumentSelectionService
  ) {}

  ngOnInit(): void {}

  initiateDocumentSelection(): void {
    const clientState = GuidGenerator.generatePseudoRandomGuid();
    this.documentsSelectionHandlerClient
      .getDocumentSelectionCallbackUrl(clientState)
      .subscribe((r) => {
        const documentSelectionClient = new DocumentSelectionClient(
          this.http,
          'https://localhost:5001'
        );
        documentSelectionClient
          .getDocumentDiscoveryData({
            callback: {
              expires_in: 3600,
              url: r.callbackUrl!,
            },
            state: clientState,
          })
          .subscribe((rr) => {
            this.documentsSelectionHandlerClient
              .openCdeDocumentSelectionPage(rr.selectDocumentsUrl)
              .subscribe(() => {
                this.documentSelectionService.referenceLink
                  .pipe(first())
                  .subscribe(() => this.onDocumentSelected.next());
              });
          });
      });
  }
}
