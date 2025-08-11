import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';

import { CdeClientHubService } from '../../services/cde-client-hub.service';
import { DocumentSelectionService } from '../../services/document-selection.service';
import { MatStepper, MatStep, MatStepLabel } from '@angular/material/stepper';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SetOpencdeServerComponent } from '../set-opencde-server/set-opencde-server.component';
import { DiscoverOpencdeApiComponent } from '../discover-opencde-api/discover-opencde-api.component';
import { AuthenticateApiComponent } from '../authenticate-api/authenticate-api.component';
import { PrepareDocumentDownloadComponent } from '../prepare-document-download/prepare-document-download.component';
import { ViewDocumentComponent } from '../view-document/view-document.component';

@Component({
  selector: 'opencde-client-upload-stepper',
  templateUrl: './upload-stepper.component.html',
  styleUrls: ['./upload-stepper.component.scss'],
  imports: [
    MatStepper,
    MatStep,
    MatStepLabel,
    SetOpencdeServerComponent,
    DiscoverOpencdeApiComponent,
    AuthenticateApiComponent,
    PrepareDocumentDownloadComponent,
    ViewDocumentComponent,
  ],
})
export class UploadStepperComponent implements OnInit, OnDestroy {
  hasSetBaseAddress = false;
  private unsubscribe: Subject<void> = new Subject<void>();

  @ViewChild('mainStepper') stepper: MatStepper | null = null;

  constructor(private cdeClientHubService: CdeClientHubService) {}

  ngOnInit(): void {
    this.cdeClientHubService.documentVersionUploadResultReceived
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(() => {
        this.stepper?.next();
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  baseAddressSelected(): void {
    if (!this.stepper) {
      return;
    }

    this.stepper.selected!.completed = true;
    this.hasSetBaseAddress = true;
    this.stepper.next();
  }
}
