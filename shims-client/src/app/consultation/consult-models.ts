export interface PatientAttendanceInvestigations {
    investigationSRequestsID: string;
    investigationsID: string;
    dateRequested: string;
    investigation: string;
}

export interface AddInvestigationRequestDto {
    patientsAttendancesID: string;
    schemeInvestigationsID: string;
}

export interface InvestigationRequestSummaryDto {
    investigationRequestsID: string;
    patientName: string;
    investigationGroupName: string;
    dateRequested: string;
    userName: string;
    isPaid: boolean;
}

export interface InvestigationPaymentDTO {
    investigationRequestsID: string;
    receipt: string;
    amount: number;
    datePaid: string | null;
    paymentTypesID: string | null;
    paymentReceiver: string | null;
    userName: string;
}

export interface AddInvestigationPaymentDto {
    investigationRequestsID: string;
    receipt: string;
    amount: number;
    paymentTypesID: string | null;
    paymentReceiver: string | null;
}

export interface UpdateInvestigationPaymentDto {
    investigationRequestsID: string;
    receipt: string;
    amount: number;
    paymentTypesID: string | null;
    paymentReceiver: string | null;
}

export interface InvestigationPaymentDetailedDto {
    investigationRequestsID: string;
    patientName: string;
    investigationGroupName: string;
    receipt: string;
    amount: number;
    datePaid: string | null;
    userName: string;
    dateRequested: string;
}

export interface InvestigationGroupDTO {
    investigationGroupsID: string;
    investigationGroup: string;
    investigationDescription: string | null;
}

export interface AddInvestigationGroupDto {
    investigationGroup: string;
    investigationDescription: string | null;
}

export interface UpdateInvestigationGroupDto {
    investigationGroupsID: string;
    investigationGroup: string;
    investigationDescription: string | null;
}

export interface InvestigationParameterDTO {
    investigationParametersID: string;
    investigationParameter: string;
    order: number;
    investigationGroupsID: string;
    investigationGroupName: string;
}

export interface AddInvestigationParameterDto {
    investigationParameter: string;
    order: number;
    investigationGroupsID: number;
}

export interface UpdateInvestigationParameterDto {
    investigationParametersID: string;
    investigationParameter: string;
    order: number;
}

export interface InvestigationResultDTO {
    investigationPaymentID: string;
    investigationParametersID: string;
    investigationParameterName: string;
    result: string;
    notes: string | null;
    dateTested: string;
    userName: string;
}

export interface AddInvestigationResultDto {
    investigationPaymentID: string;
    investigationParametersID: string;
    result: string;
    notes: string | null;
}

export interface InvestigationResultWithPaymentDto {
    investigationPaymentID: string;
    investigationGroupName: string;
    results: InvestigationResultDTO[];
    totalCost: number;
    dateCreated: string;
}
