import { LitePatientDto } from "../dtos";

/**
 * Data transfer object for vital signs information
 */
export interface VitalsDTO {
    vitalsID: string;
    patientsAttendancesID: string;
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

/**
 * Data transfer object for creating vital signs records
 */
export interface AddVitalsDto {
    patientsAttendancesID: string;
    patientsID: string;
    temperature: number;
    weight: number;
    pulse: number;
    systol: number;
    diastol: number;
    respiration: number;
    sPO2: number;
    complaints: string;
    notes: string;
}

/**
 * Data transfer object for vital signs summary
 */
export interface VitalsummaryDto {
    vitals: VitalsDTO[];
    patient: LitePatientDto;
}
