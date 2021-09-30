import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  DocumentMetadata,
  DocumentVersion,
  DocumentVersions,
  SelectedDocuments,
} from '../../generated/opencde-client';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { FileDownloadClient } from '../../generated/backend-client';
import { FileSaverService } from '../../services/file-saver.service';
import { HttpClient } from '@angular/common/http';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-view-document',
  templateUrl: './view-document.component.html',
  styleUrls: ['./view-document.component.scss'],
})
export class ViewDocumentComponent implements OnInit, OnDestroy {
  isLoading = true;
  documentVersion: DocumentVersion | null = null;
  documentMetadata: DocumentMetadata | null = null;
  documentVersions: DocumentVersions | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(
    private documentSelectionService: DocumentSelectionService,
    private http: HttpClient,
    private fileSaverService: FileSaverService,
    private fileDownloadClient: FileDownloadClient,
    private jwtTokenService: JwtTokenService
  ) {}

  ngOnInit(): void {
    this.documentSelectionService.referenceLink
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((documentReferenceUrl) => {
        this.isLoading = true;

        const accessToken =
          this.jwtTokenService.getTokenFromStorage().accessToken;

        const getProxyUrl = (actualUrl: string) =>
          `/client-proxy?accessToken=${accessToken}&targetUrl=${encodeURIComponent(
            actualUrl
          )}`;

        this.http
          .get<SelectedDocuments>(getProxyUrl(documentReferenceUrl))
          .subscribe((r) => {
            this.isLoading = false;
            if (!r.documents || r.documents.length == 0) {
              return;
            }
            this.documentVersion = r?.documents[0];

            if (this.documentVersion.links.document_version_metadata?.url) {
              this.http
                .get<DocumentMetadata>(
                  getProxyUrl(
                    this.documentVersion.links.document_version_metadata.url
                  )
                )
                .subscribe((metadata) => (this.documentMetadata = metadata));
            }

            if (this.documentVersion.links.document_versions?.url) {
              this.http
                .get<DocumentVersions>(
                  getProxyUrl(this.documentVersion.links.document_versions.url)
                )
                .subscribe((versions) => (this.documentVersions = versions));
            }
          });
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  downloadDocument(): void {
    if (!this.documentVersion) {
      return;
    }

    let downloadUrl =
      this.documentVersion?.links?.document_version_download?.url;
    if (downloadUrl) {
      this.fileDownloadClient.downloadFile(downloadUrl).subscribe((r) => {
        this.fileSaverService.saveFile(r.data, r.fileName ?? 'file');
      });
    } else {
      alert('Did not find a download url for the document.');
    }
  }
}
