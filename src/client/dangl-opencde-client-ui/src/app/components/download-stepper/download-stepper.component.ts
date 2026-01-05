import { Component, OnInit, ViewChild } from '@angular/core';

import { MatStepper, MatStep, MatStepLabel } from '@angular/material/stepper';
import { SetOpencdeServerComponent } from '../set-opencde-server/set-opencde-server.component';
import { DiscoverOpencdeApiComponent } from '../discover-opencde-api/discover-opencde-api.component';
import { AuthenticateApiComponent } from '../authenticate-api/authenticate-api.component';
import { PrepareDocumentSelectionComponent } from '../prepare-document-selection/prepare-document-selection.component';
import { ViewDocumentComponent } from '../view-document/view-document.component';

@Component({
  selector: 'opencde-client-download-stepper',
  templateUrl: './download-stepper.component.html',
  styleUrls: ['./download-stepper.component.scss'],
  imports: [
    MatStepper,
    MatStep,
    MatStepLabel,
    SetOpencdeServerComponent,
    DiscoverOpencdeApiComponent,
    AuthenticateApiComponent,
    PrepareDocumentSelectionComponent,
    ViewDocumentComponent,
  ],
})
export class DownloadStepperComponent implements OnInit {
  hasSetBaseAddress = false;

  @ViewChild('mainStepper') stepper: MatStepper | null = null;

  ngOnInit(): void {}

  baseAddressSelected(): void {
    if (!this.stepper) {
      return;
    }

    this.stepper.selected!.completed = true;
    this.hasSetBaseAddress = true;
    this.stepper.next();
  }
}
