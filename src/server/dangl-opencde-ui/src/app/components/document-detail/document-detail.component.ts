import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  DocumentGet,
  DocumentsClient,
  OpenCdeIntegrationClient,
} from '../../generated/backend-client';
import { first, takeUntil } from 'rxjs/operators';

import { ActivatedRoute } from '@angular/router';
import { CdeSessionService } from '../../services/cde-session.service';
import { JwtTokenService } from '@dangl/angular-dangl-identity-client';
import { ProgressSettings } from '../../models/progress-settings';
import { Subject } from 'rxjs';

@Component({
  selector: 'opencde-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.scss'],
})
export class DocumentDetailComponent implements OnInit, OnDestroy {
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

  constructor(
    private route: ActivatedRoute,
    private documentsClient: DocumentsClient,
    private jwtTokenService: JwtTokenService,
    private cdeSessionService: CdeSessionService,
    private openCdeIntegrationClient: OpenCdeIntegrationClient
  ) {}

  ngOnInit(): void {
    this.cdeSessionService.sessionId
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((sessionId) => (this.cdeSession = sessionId));

    this.accessToken = this.jwtTokenService.getTokenFromStorage().accessToken;

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

    this.openCdeIntegrationClient
      .setDocumentSelection(this.cdeSession, {
        documentId: this.documentId,
      })
      .subscribe((r) => {
        window.location.href = r.callbackUrl;
      });
  }
}
