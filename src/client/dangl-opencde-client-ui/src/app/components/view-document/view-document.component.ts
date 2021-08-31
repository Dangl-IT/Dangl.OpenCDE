import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  DocumentMetadata,
  DocumentReference,
  DocumentVersions,
} from '../../generated/opencde-client';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { FileSaverService } from '../../services/file-saver.service';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-view-document',
  templateUrl: './view-document.component.html',
  styleUrls: ['./view-document.component.scss'],
})
export class ViewDocumentComponent implements OnInit, OnDestroy {
  isLoading = true;
  documentReferenceData: DocumentReference | null = null;
  documentMetadata: DocumentMetadata | null = null;
  documentVersions: DocumentVersions | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private documentSelectionService: DocumentSelectionService,
    private http: HttpClient,
    private fileSaverService: FileSaverService
  ) {}

  ngOnInit(): void {
    this.documentSelectionService.referenceLink
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((documentReferenceUrl) => {
        this.isLoading = true;
        this.http
          .get<DocumentReference>(documentReferenceUrl)
          .subscribe((r) => {
            this.documentReferenceData = r;
            this.isLoading = false;

            this.http
              .get<DocumentMetadata>(
                this.documentReferenceData._links.metadata.href
              )
              .subscribe((metadata) => (this.documentMetadata = metadata));

            this.http
              .get<DocumentVersions>(
                this.documentReferenceData._links.versions.href
              )
              .subscribe((versions) => (this.documentVersions = versions));
          });
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  downloadDocument(): void {
    if (!this.documentReferenceData) {
      return;
    }

    this.http
      .get(this.documentReferenceData._links.content.href, {
        responseType: 'blob',
      })
      .subscribe((downloadResponse) => {
        const fileName = this.documentMetadata;
        this.fileSaverService.saveFile(
          downloadResponse,
          this.documentReferenceData!.file_description.name ?? 'Document'
        );
        console.log(downloadResponse);
      });
  }
}
