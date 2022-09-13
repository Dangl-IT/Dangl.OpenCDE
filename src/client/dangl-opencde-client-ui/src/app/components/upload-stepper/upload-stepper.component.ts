import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';

import { CdeClientHubService } from '../../services/cde-client-hub.service';
import { DocumentSelectionService } from '../../services/document-selection.service';
import { MatStepper } from '@angular/material/stepper';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-upload-stepper',
  templateUrl: './upload-stepper.component.html',
  styleUrls: ['./upload-stepper.component.scss'],
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
