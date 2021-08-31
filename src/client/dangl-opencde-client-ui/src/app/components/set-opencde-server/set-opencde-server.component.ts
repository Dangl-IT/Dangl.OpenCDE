import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';

@Component({
  selector: 'opencde-client-set-opencde-server',
  templateUrl: './set-opencde-server.component.html',
  styleUrls: ['./set-opencde-server.component.scss'],
})
export class SetOpencdeServerComponent implements OnInit {
  serverBaseAddress: string | null = 'https://localhost:5001/';
  @Output() onHasSelectedServerBaseAddress = new EventEmitter<void>();

  constructor(private openCdeDiscoveryService: OpenCdeDiscoveryService) {}

  ngOnInit(): void {}

  setServerBaseAddress(): void {
    if (!this.serverBaseAddress) {
      return;
    }

    this.openCdeDiscoveryService.setOpenCdeServerBaseUrl(
      this.serverBaseAddress
    );
    this.onHasSelectedServerBaseAddress.next();
  }
}
