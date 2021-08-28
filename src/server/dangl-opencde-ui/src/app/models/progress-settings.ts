import { ProgressBarMode } from '@angular/material/progress-bar';

export interface ProgressSettings {
  mode: ProgressBarMode;
  value: number;
  color: string;
  isLoading: boolean;
}
