import { Component, OnInit } from '@angular/core';
import { MatAnchor } from '@angular/material/button';
import { RouterLinkActive, RouterLink } from '@angular/router';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'opencde-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
  imports: [MatAnchor, RouterLinkActive, RouterLink, MatIcon],
})
export class SidenavComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
