import { Component, OnInit } from '@angular/core';

import { SettingsService } from '../../services/settings.service';
import { SettingsType } from '../../settings/settings.type';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'opencde-client-manage-openid-configs-modal',
  templateUrl: './manage-openid-configs-modal.component.html',
  styleUrls: ['./manage-openid-configs-modal.component.scss'],
})
export class ManageOpenidConfigsModalComponent implements OnInit {
  private currentSettings: SettingsType | null = null;
  servers: string[] = [];

  constructor(
    private settingsService: SettingsService,
    private matDialogRef: MatDialogRef<ManageOpenidConfigsModalComponent>
  ) {}

  ngOnInit(): void {
    this.loadSettings();
  }

  deleteServer(server: string): void {
    this.settingsService.deleteClientConfiguration(server);
    this.loadSettings();
  }

  selectServer(server: string): void {
    const serverConfig = this.currentSettings?.clientConfigurations[server];
    if (serverConfig) {
      this.matDialogRef.close(serverConfig);
    }
  }

  private loadSettings(): void {
    this.currentSettings = this.settingsService.getSettings();

    this.servers = [];
    Object.keys(this.currentSettings.clientConfigurations).forEach((server) => {
      this.servers.push(server);
    });
  }
}
