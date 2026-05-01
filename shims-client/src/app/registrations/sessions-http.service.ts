import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPatientDto, EditPatientDto, PatientDetailsDto, VwSessions } from '../models/registrations/IRegistrations';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SessionsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Attendances`;

    list(): Observable<VwSessions[]> {
        return this.http.get<VwSessions[]>(this.baseUrl);
    }

    patientSessions(id: string): Observable<VwSessions[]> {
        return this.http.get<VwSessions[]>(`${this.baseUrl}/${id}`);
    }

    endSession(patientAttendancesID: string): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${patientAttendancesID}/end`, {});
    }
}
