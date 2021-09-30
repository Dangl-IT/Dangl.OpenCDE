import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';

import { OpenCdeDiscoveryService } from '../../services/open-cde-discovery.service';
import { Subject } from 'rxjs';
import { VersionGet } from '../../generated/opencde-client';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'opencde-client-discover-opencde-api',
  templateUrl: './discover-opencde-api.component.html',
  styleUrls: ['./discover-opencde-api.component.scss'],
})
export class DiscoverOpencdeApiComponent implements OnInit, OnDestroy {
  isLoading = true;
  @Output() onFinishedDiscovery = new EventEmitter<void>();
  discoveredVersions: VersionGet[] | null = null;
  private unsubscribe: Subject<void> = new Subject<void>();

  constructor(private openCdeDiscoveryService: OpenCdeDiscoveryService) {}

  ngOnInit(): void {
    this.openCdeDiscoveryService.foundationsAuthentication
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(() => {
        this.isLoading = false;
      });

    this.openCdeDiscoveryService.foundationsVersions
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((r) => {
        this.isLoading = false;
        this.discoveredVersions = r;
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  goToAuthentication(): void {
    this.onFinishedDiscovery.next();
  }
}
