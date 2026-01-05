import {
  enableProdMode,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';

import { environment } from './environments/environment';
import {
  DANGL_IDENTITY_REQUEST_VALIDATOR,
  DanglIdentityModule,
} from '@dangl/angular-dangl-identity-client';
import { JwtRequestValidatorService } from './app/services/jwt-request-validator.service';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from './app/app-routing.module';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app/app.component';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    provideZoneChangeDetection(),
    importProvidersFrom(
      AppRoutingModule,
      NgDanglIconsModule,
      DanglIdentityModule,
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
