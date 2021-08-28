import { Component } from '@angular/core';
import { FooterOptions } from '@dangl/angular-material-shared';

@Component({
  selector: 'opencde-site-footer',
  templateUrl: './site-footer.component.html',
  styleUrls: ['./site-footer.component.scss'],
})
export class SiteFooterComponent {
  footerOptions: FooterOptions = {
    companyNameHtml:
      '<a href="https://www.dangl-it.de">Dangl.<strong>OpenCDE</strong> - © 2021 Dangl<strong>IT</strong> GmbH</a>',
    logoInitials: 'GD',
    copyrightUrl: 'https://www.dangl-it.com/legal-notice/',
  };
}
