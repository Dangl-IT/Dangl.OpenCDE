import { Injectable, inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private toastr = inject(ToastrService);

  public showInfoMessage(message: string) {
    this.toastr.info(message, 'Information');
  }

  public showErrorMessage(message: string) {
    this.toastr.error(message, 'Error');
  }
}
