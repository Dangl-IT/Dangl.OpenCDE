import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'opencde-projects-overview',
  templateUrl: './projects-overview.component.html',
  styleUrls: ['./projects-overview.component.scss'],
  imports: [RouterOutlet],
})
export class ProjectsOverviewComponent {
  constructor() {}
}
