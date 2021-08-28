import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private isOpenSource = new ReplaySubject<boolean>(1);
  isOpen = this.isOpenSource.asObservable();

  constructor() {
    const storedState = localStorage.getItem('SIDE_MENU_STATUS');
    if (storedState === 'opened') {
      this.isOpenSource.next(true);
    } else {
      this.isOpenSource.next(false);
    }
  }

  setSideNavStatus(isOpened: boolean): void {
    this.isOpenSource.next(isOpened);
    localStorage.setItem('SIDE_MENU_STATUS', isOpened ? 'opened' : 'closed');
  }
}
