export interface AddPatientDto {
    surname: string;
    otherNames: string;
    dateOfBirth: string | null;
    phoneNumber: string;
    sex: string;
    ghanaCard: string;
}

export interface InsuranceInformation {
    schemesID: string;
    cardID?: string;
    expiryDate?: string;
}

export interface EditPatientDto extends AddPatientDto {
    patientID: string;
}

export interface PatientPaymentSchemeDto extends InsuranceInformation {
    patientSchemesID: string;
}

export interface ListPatientsDto {
    patientsID: string;
    surname: string;
    otherNames: string;
    hospitalID: string;
    phoneNumber: string;
    ghanaCard: string;
    visitType: string;
    patientAttendancesID: string;
    dateSeen: string;
    schemes: PatientPaymentSchemeDto[];
}
