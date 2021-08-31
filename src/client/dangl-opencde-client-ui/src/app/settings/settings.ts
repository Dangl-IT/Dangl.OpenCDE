import { OpenIdConnectFlowType } from '../generated/backend-client';
import { SettingsType } from './settings.type';

export const settings: SettingsType = {
  openCdeServers: ['https://opencde-dev.dangl.dev/', 'https://localhost:5001/'],
  clientConfigurations: {
    'https://opencde-dev.dangl.dev/': {
      flow: <any>'implicit_grant',
      clientState: '',
      clientConfiguration: {
        authorizeEndpoint: '',
        clientId: '531e50f6-5b9d-4a1a-ab2c-48cec1ddba9a',
        clientSecret: '',
        requiredScope: 'dangl_opencde',
        tokenEndpoint: '',
      },
    },
    'https://localhost:5001/': {
      flow: <any>'implicit_grant',
      clientState: '',
      clientConfiguration: {
        authorizeEndpoint: '',
        clientId: 'a4e59a6a-a300-48e3-89a0-2cfbde8ce304',
        clientSecret: '',
        requiredScope: 'dangl_opencde_demo',
        tokenEndpoint: '',
      },
    },
  },
};
