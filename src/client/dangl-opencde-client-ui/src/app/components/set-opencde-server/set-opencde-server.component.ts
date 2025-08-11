import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { ManageServersModalComponent } from '../manage-servers-modal/manage-servers-modal.component';
import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { SettingsService } from '../../services/settings.service';
import { MatDialog } from '@angular/material/dialog';
import {
  MatFormField,
  MatLabel,
  MatSuffix,
} from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'opencde-client-set-opencde-server',
  templateUrl: './set-opencde-server.component.html',
  styleUrls: ['./set-opencde-server.component.scss'],
  imports: [
    MatFormField,
    MatLabel,
    MatInput,
    FormsModule,
    MatIconButton,
    MatSuffix,
    MatIcon,
    MatButton,
  ],
})
export class SetOpencdeServerComponent implements OnInit {
  serverBaseAddress: string | null = null;
  @Output() onHasSelectedServerBaseAddress = new EventEmitter<void>();

  constructor(
    private openCdeDiscoveryService: OpenCdeDiscoveryService,
    private settingsService: SettingsService,
    private matDialog: MatDialog
  ) {}

  ngOnInit(): void {
    const lastUsedServer = this.settingsService.getLastUsedCdeServerAddress();
    if (lastUsedServer) {
      this.serverBaseAddress = lastUsedServer;
    } else {
      const settings = this.settingsService.getSettings();
      if (settings.openCdeServers?.length > 0) {
        this.serverBaseAddress = settings.openCdeServers[0];
      }
    }
  }

  setServerBaseAddress(): void {
    if (!this.serverBaseAddress) {
      return;
    }

    this.openCdeDiscoveryService.setOpenCdeServerBaseUrl(
      this.serverBaseAddress
    );
    this.settingsService.setLastUsedCdeServerAddress(this.serverBaseAddress);
    this.onHasSelectedServerBaseAddress.next();
  }

  openManagementModal(): void {
    this.matDialog
      .open(ManageServersModalComponent, {
        data: this.serverBaseAddress,
      })
      .afterClosed()
      .subscribe((selectedUrl: string | null) => {
        if (selectedUrl) {
          this.serverBaseAddress = selectedUrl;
        }
      });
  }
}
