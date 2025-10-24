// This file is required by karma.conf.js and loads recursively all the .spec and framework files

import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting,
} from '@angular/platform-browser-dynamic/testing';

const originalError = console.error;
console.error = (...args: any[]) => {
  const message = args.join(' ');
  if (
    message.includes('SignalR') ||
    message.includes('Failed to complete negotiation') ||
    message.includes('NOT FOUND')
  ) {
    return;
  }
  originalError(...args);
};

// First, initialize the Angular testing environment.
getTestBed().initTestEnvironment(
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting(),
  { teardown: { destroyAfterEach: true } }
);
