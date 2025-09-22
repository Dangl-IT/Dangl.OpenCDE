import { DomSanitizer } from '@angular/platform-browser';
import { Injectable, inject } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class IconRegistry {
  private matIconRegistry = inject(MatIconRegistry);
  private domSanitizer = inject(DomSanitizer);

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
