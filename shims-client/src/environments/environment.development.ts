import { AgEnvironment } from "./IEnvironment";

export const environment: AgEnvironment = {
    AppUrl: 'https://localhost:7220/api/',
    AppName: 'KCCR-GHID Conference Room Booking System',
    Production: false,
    shortName: 'KCCR-GHID',
} as const;
