import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AngularMaterialSharedModule } from '@dangl/angular-material-shared';
import { ApiSpecComponent } from './components/api-spec/api-spec.component';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { DanglIdentityModule } from '@dangl/angular-dangl-identity-client';
import { DocumentDetailComponent } from './components/document-detail/document-detail.component';
import { DocumentSelectionStartComponent } from './components/document-selection-start/document-selection-start.component';
import { DocumentsComponent } from './components/documents/documents.component';
import { DragAndDropDirective } from './directives/drag-and-drop.directive';
import { FileSizePipe } from './pipes/file-size.pipe';
import { LoginComponent } from './components/login/login.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { NewDocumentComponent } from './components/new-document/new-document.component';
import { NewProjectComponent } from './components/new-project/new-project.component';
import { NgDanglIconsModule } from 'ng-dangl-icons';
import { NgModule } from '@angular/core';
import { ProjectsComponent } from './components/projects/projects.component';
import { ProjectsOverviewComponent } from './components/projects-overview/projects-overview.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { SiteFooterComponent } from './components/site-footer/site-footer.component';
import { SiteHeaderComponent } from './components/site-header/site-header.component';
import { UploadProgressComponent } from './components/upload-progress/upload-progress.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { WelcomeComponent } from './components/welcome/welcome.component';
import { CdeFileUploadComponent } from './components/cde-file-upload/cde-file-upload.component';

@NgModule({
  declarations: [
    AppComponent,
    SiteHeaderComponent,
    SiteFooterComponent,
    UserInfoComponent,
    LoginComponent,
    WelcomeComponent,
    ApiSpecComponent,
    SidenavComponent,
    ProjectsComponent,
    ProjectsOverviewComponent,
    DocumentsComponent,
    FileSizePipe,
    NewProjectComponent,
    NewDocumentComponent,
    UploadProgressComponent,
    DocumentDetailComponent,
    DragAndDropDirective,
    DocumentSelectionStartComponent,
    CdeFileUploadComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMaterialSharedModule,
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
    MatCheckboxModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
