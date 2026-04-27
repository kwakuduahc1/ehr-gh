import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPatientDto, EditPatientDto, PatientDetailsDto } from '../models/registrations/IRegistrations';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RegistrationsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Registrations`;

    list(): Observable<PatientDetailsDto[]> {
        return this.http.get<PatientDetailsDto[]>(this.baseUrl);
    }

    getRegistration(id: string): Observable<PatientDetailsDto> {
        return this.http.get<PatientDetailsDto>(`${this.baseUrl}/${id}`);
    }

    search(query: string): Observable<PatientDetailsDto[]> {
        return this.http.get<PatientDetailsDto[]>(`${this.baseUrl}/search`, { params: { search: query } });
    }

    register(patient: AddPatientDto): Observable<{ hid: string, pid: string }> {
        return this.http.post<{ hid: string, pid: string }>(`${this.baseUrl}`, patient);
    }

    update(patient: EditPatientDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}`, patient);
    }

    delete(id: string): Observable<string> {
        return this.http.delete<string>(`${this.baseUrl}/${id}`);
    }
}
