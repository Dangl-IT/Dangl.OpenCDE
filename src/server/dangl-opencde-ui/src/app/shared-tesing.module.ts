import { CommonModule } from '@angular/common';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NgModule } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';

@NgModule({
  exports: [CommonModule, RouterModule, NoopAnimationsModule],
  imports: [CommonModule, RouterModule.forRoot([]), NoopAnimationsModule],
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideHttpClientTesting(),
    { provide: MatDialogRef, useValue: { close: () => {} } },
    { provide: MAT_DIALOG_DATA, useValue: [] },
  ],
})
export class SharedTestingModule {}
