import { OpenIdConnectFlowType } from '../generated/backend-client';
import { SettingsType } from './settings.type';

export const settings: SettingsType = {
  openCdeServers: ['https://opencde-dev.dangl.dev/', 'https://localhost:5001/'],
  clientConfigurations: {
    'https://opencde-dev.dangl.dev/': {
      flow: OpenIdConnectFlowType.Implicit,
      clientState: '',
      clientConfiguration: {
        authorizeEndpoint: '',
        clientId: '',
        clientSecret: '',
        requiredScope: '',
        tokenEndpoint: '',
      },
    },
    'https://localhost:5001/': {
      flow: OpenIdConnectFlowType.Implicit,
      clientState: '',
      clientConfiguration: {
        authorizeEndpoint: '',
        clientId: '',
        clientSecret: '',
        requiredScope: '',
        tokenEndpoint: '',
      },
    },
  },
};
