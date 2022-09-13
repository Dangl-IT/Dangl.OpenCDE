import { Component, OnInit, ViewChild } from '@angular/core';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { MatStepper } from '@angular/material/stepper';

@Component({
  selector: 'opencde-client-download-stepper',
  templateUrl: './download-stepper.component.html',
  styleUrls: ['./download-stepper.component.scss'],
})
export class DownloadStepperComponent implements OnInit {
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
