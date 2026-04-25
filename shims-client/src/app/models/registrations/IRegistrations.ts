export interface AddPatientDto {
    surname: string;
    otherNames: string;
    dateOfBirth: string;
    ghanaCard: string | null;
    sex: string;
    schemes: string[];
    cardID: string;
    expiryDate: string;
    phoneNumber: string | null;
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
