import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';

import { ProgressSettings } from '../../models/progress-settings';
import { ProjectsClient } from '../../generated/backend-client';
import { ProjectsService } from '../../services/projects.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'opencde-new-project',
  templateUrl: './new-project.component.html',
  styleUrls: ['./new-project.component.scss'],
})
export class NewProjectComponent implements OnInit, OnDestroy {
  private unsubscribe: Subject<void> = new Subject<void>();
  newProjectForm: FormGroup;
  settingsProgress: ProgressSettings = {
    mode: 'buffer',
    value: 0,
    color: 'primary',
    isLoading: false,
  };

  constructor(
    private projectsClient: ProjectsClient,
    formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private projectsService: ProjectsService
  ) {
    this.newProjectForm = formBuilder.group({
      name: new FormControl('', [Validators.required]),
      description: new FormControl(''),
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
