import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { RouterLink } from '@angular/router';
import { StatusProvider } from '../../providers/StatusProvider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';
import { filter, switchMap } from 'rxjs';
import { LoginVm } from '../../models/IUsers';

@Component({
  selector: 'app-nav',
  imports: [
    CommonModule,
    RouterLink,
    MatIcon,
    MatButton,
    MatMenu,
    MatMenuTrigger
  ],
  templateUrl: './nav-component.html',
  styleUrl: './nav-component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavComponent {
  protected status = inject(StatusProvider);
  readonly menuOpen = signal(false);
  private snack = inject(MatSnackBar);
  diag = inject(MatDialog);

  login() {
    this.closeMenu();
    this.diag.open<LoginComponent, {}, LoginVm>(LoginComponent)
      .afterClosed()
      .pipe(
        filter(x => !!x),
        switchMap(x => this.status.login(x))
      )
      .subscribe(x => {
        this.snack.open('You have logged in', 'Dismiss')
      })
  }
  toggleMenu() {
    this.menuOpen.update(open => !open);
  }

  closeMenu() {
    this.menuOpen.set(false);
  }

  onNavLinkClick() {
    this.closeMenu();
  }

  logout() {
    this.closeMenu();
    this.status.logout();
  }
}
