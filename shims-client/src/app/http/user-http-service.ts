import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { IRoles, IUsers, URoles, IUserRoles } from '../models/IUsers';
import { environment } from '../../environments/environment';


@Injectable({ providedIn: 'root' })
export class UserHttpService {
  http = inject(HttpClient);

  roles(): Observable<IRoles[]> {
    return this.http.get<IRoles[]>(environment.AppUrl + `Roles/`)
  }

  noRoles(id: string): Observable<IRoles> {
    return this.http.get<IRoles>(environment.AppUrl + `Users/NoRoles/${id}`)
  }

  userRoles(id: string): Observable<IRoles> {
    return this.http.get<IRoles>(environment.AppUrl + `Users/Roles/${id}`)
  }

  users(): Observable<IUsers[]> {
    return this.http.get<IUsers[]>(environment.AppUrl + `Users`)
  }

  user(id: string): Observable<IUsers> {
    return this.http.get<IUsers>(environment.AppUrl + `Users/${id}`)
  }

  add(role: URoles): Observable<IUserRoles> {
    return this.http.post<IUserRoles>(environment.AppUrl + `Users`, role);
  }

  unRole(uid: string, role: string): Observable<IUserRoles> {
    return this.http.delete<IUserRoles>(environment.AppUrl + `Users/${uid}/${role}`);
  }
}
