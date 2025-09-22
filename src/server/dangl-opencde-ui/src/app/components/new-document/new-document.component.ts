import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  DocumentContentSasUploadResultGet,
  DocumentGet,
  DocumentsClient,
  SasUploadLink,
} from '../../generated/backend-client';
import {
  UntypedFormBuilder,
  UntypedFormControl,
  UntypedFormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

import { DocumentsService } from '../../services/documents.service';
import { ProgressSettings } from '../../models/progress-settings';
import { Subject } from 'rxjs';
import { first } from 'rxjs/operators';
import { UploadProgressComponent } from '../upload-progress/upload-progress.component';
import { MatDivider } from '@angular/material/divider';
import {
  MatCard,
  MatCardContent,
  MatCardActions,
} from '@angular/material/card';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatCheckbox } from '@angular/material/checkbox';
import { NgIf } from '@angular/common';
import { DragAndDropDirective } from '../../directives/drag-and-drop.directive';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { FileSizePipe } from '../../pipes/file-size.pipe';

@Component({
  selector: 'opencde-new-document',
  templateUrl: './new-document.component.html',
  styleUrls: ['./new-document.component.scss'],
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
    MatError,
    MatCheckbox,
    NgIf,
    DragAndDropDirective,
    MatIcon,
    MatButton,
    MatCardActions,
    FileSizePipe,
  ],
})
export class NewDocumentComponent implements OnInit, OnDestroy {
  private formBuilder = inject(UntypedFormBuilder);
  private route = inject(ActivatedRoute);
  private documentsService = inject(DocumentsService);
  private documentsClient = inject(DocumentsClient);
  private router = inject(Router);

  private unsubscribe: Subject<void> = new Subject<void>();
  projectId: string | null = null;
  documentCreationForm: UntypedFormGroup;
  settingsProgress: ProgressSettings = {
    mode: 'buffer',
    value: 0,
    color: 'primary',
    isLoading: false,
  };

  constructor() {
    this.documentCreationForm = this.formBuilder.group({
      name: new UntypedFormControl('', Validators.required),
      description: new UntypedFormControl(''),
      documentFile: new UntypedFormControl(null, Validators.required),
      useSasDocumentUpload: new UntypedFormControl(false),
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
        (r: DocumentGet) => {
          if (this.documentCreationForm.value.useSasDocumentUpload) {
            this.uploadDocumentViaSasLink(r);
          } else {
            this.uploadDocumentDirectlyToServer(r);
          }
        },
        () => {
          this.settingsProgress.isLoading = false;
          alert('Some error happend');
        }
      );
  }

  private uploadDocumentViaSasLink(document: DocumentGet): void {
    const documentFile: { fileName: string; data: File } =
      this.documentCreationForm!.value.documentFile;

    this.settingsProgress.isLoading = false;

    const contentType = documentFile.data.type
      ? documentFile.data.type
      : 'application/octet-stream';

    this.documentsClient
      .prepareDocumentUploadViaStorageProvider(
        document.projectId,
        document.id,
        {
          contentType: contentType,
          fileName: documentFile.data.name ?? documentFile.fileName,
          sizeInBytes: documentFile.data.size,
        }
      )
      .subscribe(
        (uploadInstructions: DocumentContentSasUploadResultGet) => {
          if (!uploadInstructions) {
            alert('Did not receive a proper upload link.');
            return;
          }

          // Here, we're not using Angular but just the regular fetch API from the browser, to demonstrate
          // how direct uploads to Azure blob storage function
          const headers: { [headerValue: string]: string } = {};
          uploadInstructions.customHeaders.forEach(
            (header) => (headers[header.name] = header.value)
          );

          fetch(uploadInstructions.sasUploadLink.uploadLink!, {
            method: 'PUT',
            body: documentFile.data,
            headers: headers,
          })
            .then(() => {
              this.documentsClient
                .markDocumentContentAsUploaded(document.projectId, document.id)
                .subscribe(
                  () => {
                    this.documentsService.forceRefresh();
                    this.settingsProgress.isLoading = false;

                    this.router.navigate(['..', 'documents', document.id], {
                      relativeTo: this.route,
                    });
                  },
                  () => {
                    this.settingsProgress.isLoading = false;
                    alert('Error during SAS upload');
                  }
                );
            })
            .catch(() => {
              this.settingsProgress.isLoading = false;
              alert('Error during SAS upload');
            });
        },
        () => {
          this.settingsProgress.isLoading = false;
          alert('Error during SAS upload');
        }
      );
  }

  private uploadDocumentDirectlyToServer(document: DocumentGet): void {
    this.documentsClient
      .uploadDocumentContent(
        this.projectId!,
        document.id,
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
