import { Component, OnInit, ViewChild } from '@angular/core';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { MatStepper } from '@angular/material/stepper';
import { MatTabGroup, MatTab } from '@angular/material/tabs';
import { DownloadStepperComponent } from '../download-stepper/download-stepper.component';
import { UploadStepperComponent } from '../upload-stepper/upload-stepper.component';

@Component({
  selector: 'opencde-client-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  imports: [
    MatTabGroup,
    MatTab,
    DownloadStepperComponent,
    UploadStepperComponent,
  ],
})
export class MainComponent implements OnInit {
  hasSetBaseAddress = false;

  @ViewChild('mainStepper') stepper: MatStepper | null = null;

  constructor(private documentSelectionService: DocumentSelectionService) {}

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
