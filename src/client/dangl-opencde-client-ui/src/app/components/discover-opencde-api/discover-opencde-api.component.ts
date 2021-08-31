import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-discover-opencde-api',
  templateUrl: './discover-opencde-api.component.html',
  styleUrls: ['./discover-opencde-api.component.scss'],
})
export class DiscoverOpencdeApiComponent implements OnInit {
  isLoading = true;
  foundFoundationsInfo = false;
  foundationBaseUrl: string | null = null;
  foundOpenCdeInfo = false;
  openCdeBaseUrl: string | null = null;
  @Output() onFinishedDiscovery = new EventEmitter<void>();

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
        const foundationInfo = r.find((rr) => rr.api_id === 'foundation');
        if (foundationInfo && foundationInfo.api_base_url) {
          this.foundFoundationsInfo = true;
          this.foundationBaseUrl = foundationInfo.api_base_url;
        }

        const openCdeInfo = r.find((rr) => rr.api_id === 'opencde');
        if (openCdeInfo && openCdeInfo.api_base_url) {
          this.foundOpenCdeInfo = true;
          this.openCdeBaseUrl = openCdeInfo.api_base_url;
        }
      });
  }

  goToAuthentication(): void {
    this.onFinishedDiscovery.next();
  }
}
