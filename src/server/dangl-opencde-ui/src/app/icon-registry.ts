import { DomSanitizer } from '@angular/platform-browser';
import { Injectable } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class IconRegistry {
  constructor(
    private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer
  ) {}

  private readonly iconPrefix = 'opencde_';

  public registerSvgIcons(): void {
    const icons = [
      {
        name: 'delete',
        path: 'delete.svg',
      },
      {
        name: 'file_upload',
        path: 'file_upload.svg',
      },
      {
        name: 'file',
        path: 'file.svg',
      },
    ];

    icons.forEach((icon) => {
      this.matIconRegistry.addSvgIcon(
        `${this.iconPrefix}${icon.name}`,
        this.domSanitizer.bypassSecurityTrustResourceUrl(
          environment.svgBaseUrl + icon.path
        )
      );
    });
  }
}
