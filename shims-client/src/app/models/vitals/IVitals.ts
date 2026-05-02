export interface VitalsDTO {
    vitalsID: string;
    PatientsAttendancesID: string;
    dateSeen: string;
    temperature: number;
    weight: number;
    pulse: number | null;
    systol: number | null;
    diastol: number | null;
    respiration: number | null;
    sPO2: number | null;
    complaints: string;
    notes: string | null;
    userName: string;
}

export interface AddVitalsDto {
    PatientsAttendancesID: string;
    temperature: number;
    weight: number;
    pulse: number | null;
    systol: number | null;
    diastol: number | null;
    respiration: number | null;
    sPO2: number | null;
    complaints: string;
    notes: string | null;
}
