import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

  public showInfoMessage(message: string) {
    this.toastr.info(message, 'Information');
  }

  public showErrorMessage(message: string) {
    this.toastr.error(message, 'Error');
  }
}
