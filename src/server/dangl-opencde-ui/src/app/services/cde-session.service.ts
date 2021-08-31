import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CdeSessionService {
  private sessionIdSource = new ReplaySubject<string>(1);
  sessionId = this.sessionIdSource.asObservable();

  setCurrentSessionId(sessionId: string): void {
    this.sessionIdSource.next(sessionId);
  }
}
