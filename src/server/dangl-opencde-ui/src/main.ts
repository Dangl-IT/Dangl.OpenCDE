import { enableProdMode, importProvidersFrom } from '@angular/core';

import { environment } from './environments/environment';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from './app/app-routing.module';
import { provideAnimations } from '@angular/platform-browser/animations';
import { DanglIdentityModule } from '@dangl/angular-dangl-identity-client';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { AppComponent } from './app/app.component';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      AppRoutingModule,
      DanglIdentityModule,
      NgDanglIconsModule
    ),
    provideAnimations(),
  ],
}).catch((err) => console.error(err));
