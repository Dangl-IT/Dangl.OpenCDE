import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NgModule } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';

@NgModule({
  exports: [RouterModule, ToastrModule, NoopAnimationsModule],
  imports: [RouterModule.forRoot([]), ToastrModule, NoopAnimationsModule],
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideHttpClientTesting(),
    { provide: MatDialogRef, useValue: { close: () => {} } },
    { provide: MAT_DIALOG_DATA, useValue: [] },
    { provide: ToastrService, useValue: {} },
  ],
})
export class SharedTestingModule {}
