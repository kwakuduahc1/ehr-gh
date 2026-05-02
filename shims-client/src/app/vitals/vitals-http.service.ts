import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VitalsDTO } from '../models/vitals/IVitals';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VitalsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Vitals`;

    list(id: string): Observable<VitalsDTO[]> {
        return this.http.get<VitalsDTO[]>(`${this.baseUrl}/${id}`);
    }

    add(dto: Omit<VitalsDTO, 'vitalsID' | 'dateSeen' | 'userName'>): Observable<void> {
        return this.http.post<void>(this.baseUrl, dto);
    }
}
