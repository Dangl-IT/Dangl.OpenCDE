import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  UntypedFormBuilder,
  UntypedFormControl,
  UntypedFormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

import { ProgressSettings } from '../../models/progress-settings';
import { ProjectsClient } from '../../generated/backend-client';
import { ProjectsService } from '../../services/projects.service';
import { Subject } from 'rxjs';
import { UploadProgressComponent } from '../upload-progress/upload-progress.component';
import { MatDivider } from '@angular/material/divider';
import {
  MatCard,
  MatCardContent,
  MatCardActions,
} from '@angular/material/card';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { NgIf } from '@angular/common';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'opencde-new-project',
  templateUrl: './new-project.component.html',
  styleUrls: ['./new-project.component.scss'],
  imports: [
    UploadProgressComponent,
    MatDivider,
    MatCard,
    FormsModule,
    ReactiveFormsModule,
    MatCardContent,
    MatFormField,
    MatLabel,
    MatInput,
    NgIf,
    MatError,
    MatCardActions,
    MatButton,
  ],
})
export class NewProjectComponent implements OnInit, OnDestroy {
  private projectsClient = inject(ProjectsClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private projectsService = inject(ProjectsService);

  private unsubscribe: Subject<void> = new Subject<void>();
  newProjectForm: UntypedFormGroup;
  settingsProgress: ProgressSettings = {
    mode: 'buffer',
    value: 0,
    color: 'primary',
    isLoading: false,
  };

  constructor() {
    const formBuilder = inject(UntypedFormBuilder);

    this.newProjectForm = formBuilder.group({
      name: new UntypedFormControl('', [Validators.required]),
      description: new UntypedFormControl(''),
    });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  createProject(): void {
    if (!this.newProjectForm.valid || this.settingsProgress.isLoading) {
      return;
    }
    this.settingsProgress.isLoading = true;
    this.projectsClient.createProject(this.newProjectForm.value).subscribe(
      (p) => {
        this.settingsProgress.isLoading = false;
        // To ensure that the new project is loaded by the service in case
        // the user navigates back to the overview, since the service internally
        // caches the pagination result
        this.projectsService.forceRefresh();
        this.router.navigate(['..', p.id], {
          relativeTo: this.route,
        });
      },
      () => (this.settingsProgress.isLoading = false)
    );
  }
}
