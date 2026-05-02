import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPatientDto, AddPatientSession, EditPatientDto, PatientDetailsDto, VwSessions } from '../models/registrations/IRegistrations';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SessionsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Attendances`;

    addSession(sess: AddPatientSession): Observable<string> {
        return this.http.post<string>(this.baseUrl, sess);
    }

    list(): Observable<VwSessions[]> {
        return this.http.get<VwSessions[]>(this.baseUrl);
    }

    patientSessions(id: string): Observable<VwSessions[]> {
        return this.http.get<VwSessions[]>(`${this.baseUrl}/${id}`);
    }

    endSession(id: string): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}`, {});
    }
}
