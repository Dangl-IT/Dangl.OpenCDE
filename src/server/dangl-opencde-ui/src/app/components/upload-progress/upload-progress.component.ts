import { Component, Input } from '@angular/core';

import { ProgressSettings } from '../../models/progress-settings';
import { MatProgressBar } from '@angular/material/progress-bar';

@Component({
  selector: 'opencde-upload-progress',
  templateUrl: './upload-progress.component.html',
  styleUrls: ['./upload-progress.component.scss'],
  imports: [MatProgressBar],
})
export class UploadProgressComponent {
  @Input() settings: ProgressSettings | null = null;
}
