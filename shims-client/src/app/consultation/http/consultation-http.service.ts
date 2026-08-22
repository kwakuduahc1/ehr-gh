import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    PatientAttendanceInvestigations,
    AddInvestigationRequestDto,
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
        return this.http.get<PatientAttendanceInvestigations[]>(`${environment.AppUrl}/investigations/${id}`);
    }

    addResult(dto: AddInvestigationResultDto): Observable<string> {
        return this.http.post<string>(`${environment.AppUrl}/results`, dto);
    }
}
