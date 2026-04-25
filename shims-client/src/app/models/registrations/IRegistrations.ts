export interface AddPatientDto {
    surname: string;
    otherNames: string;
    dateOfBirth: string;
    ghanaCard: string;
    sex: string;
    schemesID: string;
    cardID: string;
    expiryDate: string;
    phoneNumber: string;
    gender: string;
    visitType: string;
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
    cardID: string | null;
    expiryDate: string;
    visitType: string;
    attendanceDate: Date;
    patientSchemesID: string;
}
