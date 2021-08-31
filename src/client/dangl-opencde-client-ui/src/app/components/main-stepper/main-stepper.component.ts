import { Component, OnInit, ViewChild } from '@angular/core';

import { DocumentSelectionService } from '../../services/document-selection.service';
import { MatStepper } from '@angular/material/stepper';

@Component({
  selector: 'opencde-client-main-stepper',
  templateUrl: './main-stepper.component.html',
  styleUrls: ['./main-stepper.component.scss'],
})
export class MainStepperComponent implements OnInit {
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
