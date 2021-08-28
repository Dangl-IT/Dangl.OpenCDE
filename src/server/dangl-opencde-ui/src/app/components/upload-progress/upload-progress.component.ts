import { Component, Input } from '@angular/core';

import { ProgressSettings } from '../../models/progress-settings';

@Component({
  selector: 'opencde-upload-progress',
  templateUrl: './upload-progress.component.html',
  styleUrls: ['./upload-progress.component.scss'],
})
export class UploadProgressComponent {
  @Input() settings: ProgressSettings | null = null;
}
