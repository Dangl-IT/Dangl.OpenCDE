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
import { DownloadStepperComponent } from './components/download-stepper/download-stepper.component';
import { JwtRequestValidatorService } from './services/jwt-request-validator.service';
import { MainComponent } from './components/main/main.component';
import { ManageOpenidConfigsModalComponent } from './components/manage-openid-configs-modal/manage-openid-configs-modal.component';
import { ManageServersModalComponent } from './components/manage-servers-modal/manage-servers-modal.component';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyListModule as MatListModule } from '@angular/material/legacy-list';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatLegacyTabsModule as MatTabsModule } from '@angular/material/legacy-tabs';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { NgModule } from '@angular/core';
import { PrepareDocumentDownloadComponent } from './components/prepare-document-download/prepare-document-download.component';
import { PrepareDocumentSelectionComponent } from './components/prepare-document-selection/prepare-document-selection.component';
import { SetOpencdeServerComponent } from './components/set-opencde-server/set-opencde-server.component';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';
import { SiteHeaderComponent } from './components/site-header/site-header.component';
import { ToastrModule } from 'ngx-toastr';
import { UploadStepperComponent } from './components/upload-stepper/upload-stepper.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { ViewDocumentComponent } from './components/view-document/view-document.component';

@NgModule({
  declarations: [
    AppComponent,
    SiteHeaderComponent,
    SiteFooterComponent,
    UserInfoComponent,
    MainComponent,
    SetOpencdeServerComponent,
    DiscoverOpencdeApiComponent,
    AuthenticateApiComponent,
    PrepareDocumentSelectionComponent,
    ViewDocumentComponent,
    ManageServersModalComponent,
    ManageOpenidConfigsModalComponent,
    UploadStepperComponent,
    DownloadStepperComponent,
    PrepareDocumentDownloadComponent,
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
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
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
