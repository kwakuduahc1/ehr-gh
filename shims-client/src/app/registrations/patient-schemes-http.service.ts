import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EditPatientSchemeDto } from '../models/registrations/IRegistrations';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PatientSchemesHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}PatientSchemes`;

    add(scheme: EditPatientSchemeDto): Observable<string> {
        return this.http.post<string>(`${this.baseUrl}`, scheme);
    }

    edit(scheme: EditPatientSchemeDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}`, scheme);
    }

    delete(id: string): Observable<string> {
        return this.http.delete<string>(`${this.baseUrl}/${id}`);
    }
}
