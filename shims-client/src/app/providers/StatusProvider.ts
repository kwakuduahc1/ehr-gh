import { Injectable, computed, inject, signal } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LoginHttpService } from '../http/login-http-service';
import { IUsers, LoginVm } from '../models/IUsers';
import { MatSnackBar } from "@angular/material/snack-bar";
import { map, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StatusProvider {
  protected roles = signal<string[] | string>([]);
  user = signal<IUsers | null>(null);
  private snack = inject(MatSnackBar);
  private jwt = inject(JwtHelperService);
  private http = inject(LoginHttpService);

  private token = signal<string | null>(null);

  readonly isAdmin = computed<boolean>(() => {
    if (Array.isArray(this.roles()))
      return (this.roles() as [])?.some(x => x === 'Administrator')
    return false;
  });

  readonly userName = computed<string>(() => `${this.user()?.fullName}`);

  readonly isLoggedIn = computed<boolean>(() => !!this.user());

  readonly getHeader = computed(() => `Bearer ${this.token()}`);

  constructor() {
    if (typeof localStorage === 'undefined') return;
    let jwt: any = localStorage.getItem('jwt');
    if (jwt) {
      this.token.update(() => jwt);
      let tkn = this.jwt.decodeToken(jwt);
      if (!this.jwt.isTokenExpired(jwt)) {
        this.setCreds(tkn);
      }
      else {
        this.snack.open('You must log in', 'Ok');
        this.logout();
      }
    }
    else this.logout(true);
  }

  logout(initiated: boolean = false) {
    let path = location.pathname;
    if (path && path.includes('/auth')) {
      localStorage.setItem('path', path);
      return;
    }
    if (typeof localStorage !== 'undefined')
      localStorage.clear();
    this.token.update(() => null);
    this.roles.update(() => []);
    this.user.update(() => null);
    if (initiated) {
      this.snack.open('You were logged out', 'Ok');
    }
    else {
      this.snack.open("Wait to be verified", 'Dismiss');
    }
  }

  login(login: LoginVm) {
    return this.http.login(login).pipe(
      map(x => x.token),
      tap((x => this.token.set(x))),
      tap(res => this.setCreds(this.jwt.decodeToken(res))),
      tap(x => localStorage.setItem('jwt', x))
    )
  }

  private setCreds(tkn: { [x: string]: any } | null) {
    if (tkn) {
      this.roles.update(() => tkn['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
      this.user.set(
        {
          password: '',
          title: tkn['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/title'],
          id: tkn['UsersID'],
          confirmPassword: '',
          email: tkn['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
          fullName: tkn['FullName'],
          userName: tkn['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
          usersID: tkn['UsersID']
        }
      );
    }
  }
}
