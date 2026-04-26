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

export interface ListPatientsDto {
    patientID: string;
    schemesID: string;
    age: number;
    gender: string;
    fullName: string;
    scheme: string;
    hospitalID: string;
    cardID: string;
    expiryDate: string;
    visitType: string;
    attendanceDate: Date;
    patientSchemesID: string;
}
