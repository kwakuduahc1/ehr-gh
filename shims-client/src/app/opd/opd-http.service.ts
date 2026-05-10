import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PatientDetailsDto } from '../models/registrations/IRegistrations';

@Injectable({ providedIn: 'root' })
export class OpdHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Attendances`;

    list(): Observable<PatientDetailsDto[]> {
        return this.http.get<PatientDetailsDto[]>(this.baseUrl);
    }
}
