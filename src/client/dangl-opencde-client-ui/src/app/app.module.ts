import {
  DANGL_IDENTITY_REQUEST_VALIDATOR,
  DanglIdentityModule,
} from '@dangl/angular-dangl-identity-client';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AngularMaterialSharedModule } from '@dangl/angular-material-shared';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthenticateApiComponent } from './components/authenticate-api/authenticate-api.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { DiscoverOpencdeApiComponent } from './components/discover-opencde-api/discover-opencde-api.component';
import { JwtRequestValidatorService } from './services/jwt-request-validator.service';
import { MainStepperComponent } from './components/main-stepper/main-stepper.component';
import { ManageOpenidConfigsModalComponent } from './components/manage-openid-configs-modal/manage-openid-configs-modal.component';
import { ManageServersModalComponent } from './components/manage-servers-modal/manage-servers-modal.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { NgModule } from '@angular/core';
import { PrepareDocumentSelectionComponent } from './components/prepare-document-selection/prepare-document-selection.component';
import { SetOpencdeServerComponent } from './components/set-opencde-server/set-opencde-server.component';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';
import { SiteHeaderComponent } from './components/site-header/site-header.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { ViewDocumentComponent } from './components/view-document/view-document.component';

@NgModule({
  declarations: [
    AppComponent,
    SiteHeaderComponent,
    SiteFooterComponent,
    UserInfoComponent,
    MainStepperComponent,
    SetOpencdeServerComponent,
    DiscoverOpencdeApiComponent,
    AuthenticateApiComponent,
    PrepareDocumentSelectionComponent,
    ViewDocumentComponent,
    ManageServersModalComponent,
    ManageOpenidConfigsModalComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatIconModule,
    MatMenuModule,
    NgDanglIconsModule,
    AngularMaterialSharedModule,
    DanglIdentityModule,
    MatButtonModule,
    BrowserAnimationsModule,
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
  ],
  providers: [
    {
      provide: DANGL_IDENTITY_REQUEST_VALIDATOR,
      useClass: JwtRequestValidatorService,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
