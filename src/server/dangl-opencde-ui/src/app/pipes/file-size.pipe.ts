import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileSize',
})
export class FileSizePipe implements PipeTransform {
  transform(value: number): string {
    if (value < 0) {
      return '';
    }
    if (value <= 1024) {
      return `${value} B`;
    }
    if (value <= 1024 * 1024) {
      return `${(value / 1024).toFixed(2).replace('.', ',')} KB`;
    }
    if (value <= 1024 * 1024 * 1024) {
      return `${(value / 1024 / 1024).toFixed(2).replace('.', ',')} MB`;
    }
    if (value <= 1024 * 1024 * 1024 * 1024) {
      return `${(value / 1024 / 1024 / 1024).toFixed(2).replace('.', ',')} GB`;
    }

    return `${value}b`;
  }
}
