import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    PatientAttendanceInvestigations,
    AddInvestigationRequestDto,
    SchemeInvestigationDTO,
    InvestigationRequestSummaryDto,
    InvestigationPaymentDTO,
    AddInvestigationPaymentDto,
    UpdateInvestigationPaymentDto,
    InvestigationPaymentDetailedDto,
    InvestigationGroupDTO,
    AddInvestigationGroupDto,
    UpdateInvestigationGroupDto,
    InvestigationParameterDTO,
    AddInvestigationParameterDto,
    UpdateInvestigationParameterDto,
    InvestigationResultDTO,
    AddInvestigationResultDto,
    InvestigationResultWithPaymentDto,
} from '../consult-models';
import { PatientDetailsDto } from '../../models/registrations/IRegistrations';
import { PatientConsultationDto } from '../../models/consultation/history';

@Injectable({ providedIn: 'root' })
export class ConsultationHttpService {
    private readonly http = inject(HttpClient);

    pending(): Observable<PatientDetailsDto[]> {
        return this.http.get<PatientDetailsDto[]>(`${environment.AppUrl}/Consulting/Pending`);
    }
    // Investigation Requests
    getInvestigationsByAttendance(id: string): Observable<PatientAttendanceInvestigations[]> {
        return this.http.get<PatientAttendanceInvestigations[]>(`${environment.AppUrl}Consultations/investigations/${id}`);
    }

    getInvestigationsByScheme(id: string): Observable<SchemeInvestigationDTO[]> {
        return this.http.get<SchemeInvestigationDTO[]>(`${environment.AppUrl}SchemeInvestigations/scheme/${id}`);
    }

    addInvestigationRequest(dto: AddInvestigationRequestDto): Observable<unknown> {
        return this.http.post(`${environment.AppUrl}Consultations/lab-request`, dto);
    }

    addResult(dto: AddInvestigationResultDto): Observable<string> {
        return this.http.post<string>(`${environment.AppUrl}/results`, dto);
    }
}
