/**
 * Lightweight patient data transfer object
 */
export interface LitePatientDto {
    patientsID: string;
    hospitalID: string;
    fullName: string;
    sex: string;
    age: number;
    visitType: string;
}
