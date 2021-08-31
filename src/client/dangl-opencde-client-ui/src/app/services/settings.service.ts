import { Injectable } from '@angular/core';
import { OpenIdConnectAuthenticationParameters } from '../generated/backend-client';
import { SettingsType } from '../settings/settings.type';
import { settings as defaultSettings } from '../settings/settings';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  private readonly STORAGE_KEY = 'OpenCdeClientSettings';
  private readonly LAST_USED_CDE_SERVER_STORAGE_KEY = 'LastUsedCdeServer';

  constructor() {}

  getLastUsedCdeServerAddress(): string | null {
    return localStorage.getItem(this.LAST_USED_CDE_SERVER_STORAGE_KEY);
  }

  setLastUsedCdeServerAddress(cdeServer: string | null): void {
    if (cdeServer) {
      localStorage.setItem(this.LAST_USED_CDE_SERVER_STORAGE_KEY, cdeServer);
    } else {
      localStorage.removeItem(this.LAST_USED_CDE_SERVER_STORAGE_KEY);
    }
  }

  getSettings(): SettingsType {
    var storage = localStorage.getItem(this.STORAGE_KEY);
    if (storage) {
      return JSON.parse(storage);
    }

    // We'll just persist the default settings otherwise
    this.saveSettings(defaultSettings);
    return defaultSettings;
  }

  saveCdeServer(cdeServerAddress: string): void {
    cdeServerAddress = cdeServerAddress.toLowerCase();
    const currentSettings = this.getSettings();
    if (
      currentSettings.openCdeServers.findIndex((s) => s === cdeServerAddress) >
      -1
    ) {
      return;
    }

    currentSettings.openCdeServers.push(cdeServerAddress);
    this.saveSettings(currentSettings);
  }

  saveClientConfiguration(
    cdeServerAddress: string,
    clientConfiguarion: OpenIdConnectAuthenticationParameters
  ): void {
    cdeServerAddress = cdeServerAddress.toLowerCase();
    const currentSettings = this.getSettings();
    currentSettings.clientConfigurations[cdeServerAddress] = clientConfiguarion;
    this.saveSettings(currentSettings);
  }

  deleteCdeServer(cdeServerAddress: string): void {
    cdeServerAddress = cdeServerAddress.toLowerCase();
    const currentSettings = this.getSettings();

    currentSettings.openCdeServers = currentSettings.openCdeServers.filter(
      (s) => s !== cdeServerAddress
    );

    this.saveSettings(currentSettings);
  }

  deleteClientConfiguration(cdeServerAddress: string): void {
    cdeServerAddress = cdeServerAddress.toLowerCase();
    const currentSettings = this.getSettings();
    if (currentSettings.clientConfigurations[cdeServerAddress]) {
      delete currentSettings.clientConfigurations[cdeServerAddress];
      this.saveSettings(currentSettings);
    }
  }

  private saveSettings(settings: SettingsType): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(settings));
  }
}
