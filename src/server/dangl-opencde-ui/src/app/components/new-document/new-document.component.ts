import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';

import { DocumentsClient } from '../../generated/backend-client';
import { DocumentsService } from '../../services/documents.service';
import { ProgressSettings } from '../../models/progress-settings';
import { Subject } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
  selector: 'opencde-new-document',
  templateUrl: './new-document.component.html',
  styleUrls: ['./new-document.component.scss'],
})
export class NewDocumentComponent implements OnInit, OnDestroy {
  private unsubscribe: Subject<void> = new Subject<void>();
  projectId: string | null = null;
  documentCreationForm: FormGroup;
  settingsProgress: ProgressSettings = {
    mode: 'buffer',
    value: 0,
    color: 'primary',
    isLoading: false,
  };

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private documentsService: DocumentsService,
    private documentsClient: DocumentsClient,
    private router: Router
  ) {
    this.documentCreationForm = this.formBuilder.group({
      name: new FormControl('', Validators.required),
      description: new FormControl(''),
      documentFile: new FormControl(null, Validators.required),
    });
  }

  ngOnInit(): void {
    this.route.params.pipe(first()).subscribe((p) => {
      if (p.projectId) {
        this.projectId = p.projectId;
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  createDocument(): void {
    if (
      !this.projectId ||
      this.settingsProgress.isLoading ||
      !this.documentCreationForm
    ) {
      return;
    }

    this.settingsProgress.isLoading = true;

    this.documentsClient
      .uploadDocumentMetadataForProject(this.projectId, {
        name: this.documentCreationForm.value.name,
        description: this.documentCreationForm.value.description,
      })
      .subscribe(
        (r) => {
          this.documentsClient
            .uploadDocumentContent(
              this.projectId!,
              r.id,
              this.documentCreationForm!.value.documentFile
            )
            .subscribe(
              (r) => {
                this.settingsProgress.isLoading = false;

                // To ensure that the new document is loaded by the service in case
                // the user navigates back to the overview, since the service internally
                // caches the pagination result
                this.documentsService.forceRefresh();
                this.router.navigate(['..', 'documents', r.id], {
                  relativeTo: this.route,
                });
              },
              () => {
                this.settingsProgress.isLoading = false;
                alert('Some error happened');
              }
            );
        },
        () => {
          this.settingsProgress.isLoading = false;
          alert('Some error happend');
        }
      );
  }

  onFileDropped(fileList: FileList) {
    const file = fileList[0];
    this.setFile(file);
  }

  fileBrowseHandler(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length) {
      const file = target.files[0];
      this.setFile(file);
    }
  }

  deleteFile(field: string): void {
    this.documentCreationForm!.get(field)?.setValue(null);
  }

  private setFile(file: File): void {
    this.documentCreationForm!.patchValue({
      documentFile: { fileName: file.name, data: file },
    });
  }
}
