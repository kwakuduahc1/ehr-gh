import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VitalsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Vitals`;

    list(): Observable<unknown[]> {
        return this.http.get<unknown[]>(this.baseUrl);
    }
}
