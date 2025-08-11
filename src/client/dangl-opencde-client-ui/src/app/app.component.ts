import { Component } from '@angular/core';
import { SiteHeaderComponent } from './components/site-header/site-header.component';
import { RouterOutlet } from '@angular/router';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';

@Component({
  selector: 'opencde-client-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [SiteHeaderComponent, RouterOutlet, SiteFooterComponent],
})
export class AppComponent {
  title = 'dangl-opencde-client-ui';
}
