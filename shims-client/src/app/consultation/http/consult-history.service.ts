import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { AddPatientConsultationDto, PatientConsultationDto } from '../consult-models';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConsultHistoryHttpService {
    private readonly http = inject(HttpClient);

    history(id: string, take: number = 0, skip: number = 15): Observable<PatientConsultationDto[]> {
        return this.http.get<PatientConsultationDto[]>(`${environment.AppUrl}Consultations/${id}/${take}/${skip}`);
    }

    addHistory(hist: AddPatientConsultationDto): Observable<string> {
        return this.http.post<string>(`${environment.AppUrl}Consultations`, hist);
    }

    editHistory(hist: PatientConsultationDto): Observable<string> {
        return this.http.put<string>(`${environment.AppUrl}Consultations`, hist);
    }
}
