export interface AddPatientDto {
    surname: string;
    otherNames: string;
    dateOfBirth: string | null;
    phoneNumber: string;
    sex: string;
    ghanaCard: string;
}

export interface InsuranceDetails {
    schemesID: string;
    cardID?: string | null;
    expiryDate?: string | null;
    patientSchemesID: string;
    schemeName?: string | null;
    coverage?: string | null;
}

export interface EditPatientDto extends AddPatientDto {
    patientsID: string;
    hospitalID?: string | null;
}

export interface PatientDetailsDto {
    patientsID: string;
    fullName: string;
    hospitalID: string;
    phoneNumber: string;
    ghanaCard: string;
    visitType: string;
    patientAttendancesID: string;
    dateSeen: string;
    sex: string;
    dateOfBirth: string;
    age: number;
    schemes: InsuranceDetails[];
}
