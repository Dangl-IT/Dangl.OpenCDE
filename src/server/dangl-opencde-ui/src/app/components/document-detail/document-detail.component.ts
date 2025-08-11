import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  DocumentGet,
  DocumentsClient,
  OpenCdeDownloadIntegrationClient,
} from '../../generated/backend-client';
import { first, takeUntil } from 'rxjs/operators';

import { CdeSessionService } from '../../services/cde-session.service';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { ProgressSettings } from '../../models/progress-settings';
import { Subject } from 'rxjs';
import { NgIf } from '@angular/common';
import { UploadProgressComponent } from '../upload-progress/upload-progress.component';
import { MatAnchor, MatButton } from '@angular/material/button';

@Component({
  selector: 'opencde-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.scss'],
  imports: [NgIf, UploadProgressComponent, MatAnchor, MatButton],
})
export class DocumentDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private documentsClient = inject(DocumentsClient);
  private jwtTokenService = inject(JwtTokenService);
  private cdeSessionService = inject(CdeSessionService);
  private openCdeDownloadIntegrationClient = inject(
    OpenCdeDownloadIntegrationClient
  );
  private router = inject(Router);

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
  cdeSession: string | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  ngOnInit(): void {
    this.cdeSessionService.sessionId
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((sessionId) => (this.cdeSession = sessionId));

    this.accessToken = this.jwtTokenService.getTokenFromStorage()?.accessToken;

    this.route.params.pipe(first()).subscribe((p) => {
      if (p.projectId && p.documentId) {
        this.projectId = p.projectId;
        this.documentId = p.documentId;
        this.loadDocumentData();
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
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

  sendDocumentToClient(): void {
    if (!this.cdeSession || !this.documentId) {
      return;
    }

    this.openCdeDownloadIntegrationClient
      .setDocumentSelection(this.cdeSession, {
        documentId: this.documentId,
      })
      .subscribe((r) => {
        window.location.href = r.callbackUrl;
      });
  }

  deleteDocument(): void {
    if (this.document) {
      this.documentsClient
        .deleteDocument(this.document?.projectId, this.document?.id)
        .subscribe(() => {
          this.router.navigate(['projects', this.projectId]);
        });
    }
  }
}
