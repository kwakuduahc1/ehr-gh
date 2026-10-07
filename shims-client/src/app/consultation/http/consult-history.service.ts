import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { AddPatientConsultationDto, PatientConsultationDto } from '../consult-models';
import { environment } from '../../../environments/environment';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConsultHistoryHttpService {
    private readonly http = inject(HttpClient);

    history(id: string, take: number = 15, skip: number = 0): Observable<PatientConsultationDto[]> {
        return this.http.get<PatientConsultationDto[]>(`${environment.AppUrl}Consultations/${id}/${take}/${skip}`)
            .pipe(
            // map(r => {
            //     r.map(p => p.avpu = p.avpu ? JSON.parse(p.avpu as unknown as string) : { alert: NaN, verbal: NaN, pain: NaN, responsive: NaN });
            //     r.map(p => p.gcs = p.gcs ? JSON.parse(p.gcs as unknown as string) : { eyeOpening: NaN, verbalResponse: NaN, motorResponse: NaN });
            //     return r;
            // })
        );
    }

    addHistory(hist: AddPatientConsultationDto): Observable<string> {
        return this.http.post<string>(`${environment.AppUrl}Consultations`, hist);
    }

    editHistory(hist: PatientConsultationDto): Observable<string> {
        return this.http.put<string>(`${environment.AppUrl}Consultations`, hist);
    }
}
