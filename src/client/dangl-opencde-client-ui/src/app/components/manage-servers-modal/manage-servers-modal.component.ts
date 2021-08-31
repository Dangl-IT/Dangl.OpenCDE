import { Component, Inject, OnInit } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SettingsService } from '../../services/settings.service';

@Component({
  selector: 'opencde-client-manage-servers-modal',
  templateUrl: './manage-servers-modal.component.html',
  styleUrls: ['./manage-servers-modal.component.scss'],
})
export class ManageServersModalComponent implements OnInit {
  savedServers: string[] = [];
  canSaveCurrentServer = false;

  constructor(
    private settingsService: SettingsService,
    private matDialogRef: MatDialogRef<ManageServersModalComponent>,
    @Inject(MAT_DIALOG_DATA) public currentServer: string | null
  ) {}

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
