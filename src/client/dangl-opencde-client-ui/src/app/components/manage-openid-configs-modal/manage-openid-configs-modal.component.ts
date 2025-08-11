import { Component, OnInit, inject } from '@angular/core';

import { SettingsService } from '../../services/settings.service';
import { SettingsType } from '../../settings/settings.type';
import { MatDialogRef } from '@angular/material/dialog';
import { MatList, MatListItem } from '@angular/material/list';
import { NgFor } from '@angular/common';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'opencde-client-manage-openid-configs-modal',
  templateUrl: './manage-openid-configs-modal.component.html',
  styleUrls: ['./manage-openid-configs-modal.component.scss'],
  imports: [MatList, NgFor, MatListItem, MatIconButton, MatIcon, MatButton],
})
export class ManageOpenidConfigsModalComponent implements OnInit {
  private settingsService = inject(SettingsService);
  private matDialogRef =
    inject<MatDialogRef<ManageOpenidConfigsModalComponent>>(MatDialogRef);

  private currentSettings: SettingsType | null = null;
  servers: string[] = [];

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
