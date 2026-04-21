import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { StatusProvider } from '../../providers/StatusProvider';

@Component({
  selector: 'app-nav',
  imports: [
    CommonModule,
    RouterLink,
    MatIcon
  ],
  templateUrl: './nav-component.html',
  styleUrl: './nav-component.scss',
})
export class NavComponent {
  protected status = inject(StatusProvider);
  readonly menuOpen = signal(false);

  toggleMenu() {
    this.menuOpen.update(open => !open);
  }

  closeMenu() {
    this.menuOpen.set(false);
  }

  onNavLinkClick() {
    this.closeMenu();
  }
}
