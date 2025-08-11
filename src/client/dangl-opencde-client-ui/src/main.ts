import { enableProdMode, importProvidersFrom } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { environment } from './environments/environment';
import {
  DANGL_IDENTITY_REQUEST_VALIDATOR,
  DanglIdentityModule,
} from '@dangl/angular-dangl-identity-client';
import { JwtRequestValidatorService } from './app/services/jwt-request-validator.service';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from './app/app-routing.module';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import {
  HeaderComponent,
  FooterComponent,
} from '@dangl/angular-material-shared';
import { MatButtonModule } from '@angular/material/button';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';
import { MatListModule } from '@angular/material/list';
import { ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app/app.component';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      BrowserModule,
      AppRoutingModule,
      MatIconModule,
      MatMenuModule,
      NgDanglIconsModule,
      HeaderComponent,
      FooterComponent,
      DanglIdentityModule,
      MatButtonModule,
      MatStepperModule,
      MatInputModule,
      FormsModule,
      MatProgressSpinnerModule,
      MatFormFieldModule,
      ReactiveFormsModule,
      MatSelectModule,
      MatTabsModule,
      MatDialogModule,
      MatListModule,
      ToastrModule.forRoot({
        positionClass: 'toast-bottom-right',
        preventDuplicates: true,
      })
    ),
    {
      provide: DANGL_IDENTITY_REQUEST_VALIDATOR,
      useClass: JwtRequestValidatorService,
    },
    provideAnimations(),
  ],
}).catch((err) => console.error(err));
