import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DocumentSelectionService {
  private referenceLinkSource = new Subject<string>();
  referenceLink = this.referenceLinkSource.asObservable();

  constructor() {}

  setSelectedDocument(referenceLink: string): void {
    this.referenceLinkSource.next(referenceLink);
  }
}
