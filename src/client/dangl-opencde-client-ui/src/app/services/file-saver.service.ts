import { Injectable } from '@angular/core';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root',
})
export class FileSaverService {
  saveFile(data: Blob, fileName: string): void {
    saveAs(data, fileName);
  }
}
