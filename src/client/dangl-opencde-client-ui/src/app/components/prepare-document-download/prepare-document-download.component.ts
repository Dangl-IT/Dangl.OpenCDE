import { Component, OnInit } from '@angular/core';
import { first, map } from 'rxjs/operators';

import { DocumentsUploadHandlerClient } from '../../generated/backend-client';
import { GuidGenerator } from '@dangl/angular-material-shared/guid-generator';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'opencde-client-prepare-document-download',
  templateUrl: './prepare-document-download.component.html',
  styleUrls: ['./prepare-document-download.component.scss'],
})
export class PrepareDocumentDownloadComponent implements OnInit {
  constructor(
    private documentsUploadHandlerClient: DocumentsUploadHandlerClient,
    private openCdeDiscoveryService: OpenCdeDiscoveryService,
    private jwtTokenService: JwtTokenService
  ) {}

  ngOnInit(): void {}

  onFilesSelected(files: FileList | null): void {
    if (files == null) {
      return;
    }

    // We just do a single file upload in the demo client
    const file = files.item(0);
    if (file == null) {
      return;
    }

    this.initiateDocumentUpload(file);
  }

  initiateDocumentUpload(file: File): void {
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
        this.documentsUploadHandlerClient
          .prepareDocumentUploadAndOpenSystemBrowser({
            accessToken: values.token,
            openCdeBaseUrl: values?.baseUrl ?? '',
            clientState: clientState,
            files: [
              {
                fileName: file.name,
                fileSizeInBytes: file.size,
                sessionFileId: GuidGenerator.generatePseudoRandomGuid(),
                filePath: (file as any).path,
              },
            ],
          })
          .subscribe((r) => {
            // Everything is now handled by the backend of the client
          });
      });
  }
}
