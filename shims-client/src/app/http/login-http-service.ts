import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChangePasswordVm, ILogin, IRoles, IUsers, LoginVm, RegisterVm, URoles } from '../models/IUsers';
import { environment } from '../../environments/environment';


@Injectable({ providedIn: 'root' })
export class LoginHttpService {
  http = inject(HttpClient);

  login(st: LoginVm): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${environment.AppUrl}Auth/Login`, st);
  }

  signout(): Observable<any> {
    return this.http.post(`${environment.AppUrl}Auth/Signout`, {});
  }

  change(user: ChangePasswordVm) {
    return this.http.post<null>(`${environment.AppUrl}Auth/ChangePassword`, user);
  }

  register(usr: RegisterVm): Observable<URoles> {
    return this.http.post<URoles>(`${environment.AppUrl}Auth/Register`, usr);
  }

  delete(id: string): Observable<IUsers> {
    return this.http.delete<IUsers>(environment.AppUrl + `Users/RemoveUser/${id}`);
  }

  roles(): Observable<IRoles[]> {
    return this.http.get<IRoles[]>(environment.AppUrl + `Roles/`)
  }
}
