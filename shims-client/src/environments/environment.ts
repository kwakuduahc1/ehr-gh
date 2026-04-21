import { AgEnvironment } from "./IEnvironment";

export const environment: AgEnvironment = {
    AppUrl: 'http://server/api/',
    AppName: 'KCCR-GHID Conference Room Booking System',
    shortName: 'KCCR-GHID',
    Production: false,
} as const;
