import { RouterModule, Routes } from '@angular/router';

import { ApiSpecComponent } from './components/api-spec/api-spec.component';
import { CdeFileUploadComponent } from './components/cde-file-upload/cde-file-upload.component';
import { DocumentDetailComponent } from './components/document-detail/document-detail.component';
import { DocumentSelectionStartComponent } from './components/document-selection-start/document-selection-start.component';
import { DocumentsComponent } from './components/documents/documents.component';
import { LoginComponent } from './components/login/login.component';
import { NewDocumentComponent } from './components/new-document/new-document.component';
import { NewProjectComponent } from './components/new-project/new-project.component';
import { NgModule } from '@angular/core';
import { ProjectsComponent } from './components/projects/projects.component';
import { ProjectsOverviewComponent } from './components/projects-overview/projects-overview.component';
import { WelcomeComponent } from './components/welcome/welcome.component';

const routes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'api-spec',
    component: ApiSpecComponent,
  },
  {
    path: 'opencde-select-documents',
    component: DocumentSelectionStartComponent,
  },
  {
    path: 'opencde-upload-documents',
    component: CdeFileUploadComponent,
  },
  {
    path: 'projects',
    component: ProjectsOverviewComponent,
    children: [
      {
        path: '',
        component: ProjectsComponent,
      },
      {
        path: 'create',
        component: NewProjectComponent,
      },
      {
        path: ':projectId',
        component: DocumentsComponent,
      },
      {
        path: ':projectId/create-document',
        component: NewDocumentComponent,
      },
      {
        path: ':projectId/documents/:documentId',
        component: DocumentDetailComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
