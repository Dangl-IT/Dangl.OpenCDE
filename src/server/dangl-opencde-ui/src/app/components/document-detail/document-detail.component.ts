import { Component, OnInit } from '@angular/core';
import { DocumentGet, DocumentsClient } from '../../generated/backend-client';

import { ActivatedRoute } from '@angular/router';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { ProgressSettings } from '../../models/progress-settings';
import { first } from 'rxjs/operators';

@Component({
  selector: 'opencde-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.scss'],
})
export class DocumentDetailComponent implements OnInit {
  projectId: string | null = null;
  documentId: string | null = null;
  document: DocumentGet | null = null;
  accessToken: string | null = null;
  settingsProgress: ProgressSettings = {
    mode: 'buffer',
    value: 0,
    color: 'primary',
    isLoading: true,
  };

  constructor(
    private route: ActivatedRoute,
    private documentsClient: DocumentsClient,
    private jwtTokenService: JwtTokenService
  ) {}

  ngOnInit(): void {
    this.accessToken = this.jwtTokenService.getTokenFromStorage().accessToken;

    this.route.params.pipe(first()).subscribe((p) => {
      if (p.projectId && p.documentId) {
        this.projectId = p.projectId;
        this.documentId = p.documentId;
        this.loadDocumentData();
      }
    });
  }

  private loadDocumentData(): void {
    if (!this.projectId || !this.documentId) {
      return;
    }

    this.documentsClient
      .getDocumentById(this.projectId, this.documentId)
      .subscribe((d) => {
        this.document = d;
        this.settingsProgress.isLoading = false;
      });
  }
}
