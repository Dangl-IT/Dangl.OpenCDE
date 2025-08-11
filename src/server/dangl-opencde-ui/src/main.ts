import { enableProdMode, importProvidersFrom } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { environment } from './environments/environment';
import {
  HeaderComponent,
  FooterComponent,
} from '@dangl/angular-material-shared';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from './app/app-routing.module';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { DanglIdentityModule } from '@dangl/angular-dangl-identity-client';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AppComponent } from './app/app.component';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      HeaderComponent,
      FooterComponent,
      BrowserModule,
      AppRoutingModule,
      MatSidenavModule,
      MatButtonModule,
      DanglIdentityModule,
      NgDanglIconsModule,
      MatMenuModule,
      MatIconModule,
      MatCardModule,
      MatTableModule,
      MatPaginatorModule,
      FormsModule,
      MatInputModule,
      MatSortModule,
      MatDividerModule,
      MatFormFieldModule,
      ReactiveFormsModule,
      MatProgressBarModule,
      MatProgressSpinnerModule,
      MatCheckboxModule
    ),
    provideAnimations(),
  ],
}).catch((err) => console.error(err));
