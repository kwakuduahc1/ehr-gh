import { Injectable, inject } from '@angular/core';
import { StatusProvider } from '../providers/StatusProvider';
import { MatSnackBar } from "@angular/material/snack-bar";
import { RouteProvider } from '../providers/RouteProvider';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class LoginGuard {

  snack = inject(MatSnackBar)
  status = inject(StatusProvider);
  path = inject(Router);
  route = inject(RouteProvider);
  canActivate() {
    this.route.path.update(() => this.path.url);
    if (this.status.isLoggedIn())
      return true;
    else {
      this.snack.open('You have no access to this application');
      return false;
    }
  }
}
