import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPatientDto, EditPatientDto, ListPatientsDto } from '../models/registrations/IRegistrations';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RegistrationsHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Registrations`;

    list(): Observable<ListPatientsDto[]> {
        return this.http.get<ListPatientsDto[]>(this.baseUrl);
    }

    getRegistration(id: string): Observable<ListPatientsDto> {
        return this.http.get<ListPatientsDto>(`${this.baseUrl}/${id}`);
    }

    search(query: string): Observable<ListPatientsDto[]> {
        return this.http.get<ListPatientsDto[]>(`${this.baseUrl}/search`, { params: { search: query } });
    }

    register(patient: AddPatientDto): Observable<{ hosid: string, pid: string }> {
        return this.http.post<{ hosid: string, pid: string }>(`${this.baseUrl}/register`, patient);
    }

    update(patient: EditPatientDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}`, patient);
    }

    delete(id: string): Observable<string> {
        return this.http.delete<string>(`${this.baseUrl}/${id}`);
    }
}
