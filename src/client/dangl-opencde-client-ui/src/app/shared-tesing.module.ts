import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NgModule } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RouterModule } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';

@NgModule({
  exports: [CommonModule, RouterModule, ToastrModule],
  imports: [CommonModule, RouterModule.forRoot([]), ToastrModule],
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideHttpClientTesting(),
    { provide: MatDialogRef, useValue: { close: () => {} } },
    { provide: MAT_DIALOG_DATA, useValue: [] },
    { provide: ToastrService, useValue: {} }
  ]
})
export class SharedTestingModule {}
