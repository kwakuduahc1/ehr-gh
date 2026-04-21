import { AgEnvironment } from "./IEnvironment";

export const environment: AgEnvironment = {
    AppUrl: '/api/',
    AppName: 'KCCR-GHID Conference Room Booking System',
    Production: true,
    shortName: 'KCCR-GHID',
} as const;
