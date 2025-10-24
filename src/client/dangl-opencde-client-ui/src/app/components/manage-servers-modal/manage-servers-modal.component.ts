import { Component, OnInit, inject } from '@angular/core';

import { SettingsService } from '../../services/settings.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatList, MatListItem } from '@angular/material/list';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'opencde-client-manage-servers-modal',
  templateUrl: './manage-servers-modal.component.html',
  styleUrls: ['./manage-servers-modal.component.scss'],
  imports: [MatButton, MatList, MatIconButton, MatIcon, MatListItem],
})
export class ManageServersModalComponent implements OnInit {
  private settingsService = inject(SettingsService);
  private matDialogRef =
    inject<MatDialogRef<ManageServersModalComponent>>(MatDialogRef);
  currentServer = inject(MAT_DIALOG_DATA);

  savedServers: string[] = [];
  canSaveCurrentServer = false;

  ngOnInit(): void {
    this.loadServersFromSettings();
  }

  saveCurrentServer(): void {
    if (this.currentServer) {
      this.settingsService.saveCdeServer(this.currentServer);
      this.loadServersFromSettings();
    }
  }

  selectServer(server: string): void {
    this.matDialogRef.close(server);
  }

  deleteServer(server: string): void {
    this.settingsService.deleteCdeServer(server);
    this.loadServersFromSettings();
  }

  private loadServersFromSettings(): void {
    this.savedServers = this.settingsService.getSettings().openCdeServers;
    this.canSaveCurrentServer =
      !!this.currentServer &&
      this.savedServers.indexOf(this.currentServer.toLowerCase()) === -1;
  }
}
