import { OpenIdConnectAuthenticationParameters } from '../generated/backend-client';

export interface SettingsType {
  openCdeServers: string[];
  clientConfigurations: {
    [openCdeServer: string]: OpenIdConnectAuthenticationParameters;
  };
}
