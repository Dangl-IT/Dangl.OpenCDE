import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { VersionGet } from '../../generated/opencde-client';
import { first } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-discover-opencde-api',
  templateUrl: './discover-opencde-api.component.html',
  styleUrls: ['./discover-opencde-api.component.scss'],
})
export class DiscoverOpencdeApiComponent implements OnInit {
  isLoading = true;
  @Output() onFinishedDiscovery = new EventEmitter<void>();
  discoveredVersions: VersionGet[] | null = null;

  constructor(private openCdeDiscoveryService: OpenCdeDiscoveryService) {}

  ngOnInit(): void {
    this.openCdeDiscoveryService.foundationsAuthentication
      .pipe(first())
      .subscribe(() => {
        this.isLoading = false;
      });

    this.openCdeDiscoveryService.foundationsVersions
      .pipe(first())
      .subscribe((r) => {
        this.isLoading = false;
        this.discoveredVersions = r;
      });
  }

  goToAuthentication(): void {
    this.onFinishedDiscovery.next();
  }
}
