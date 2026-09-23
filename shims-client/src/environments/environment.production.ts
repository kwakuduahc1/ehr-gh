import { AgEnvironment } from "./IEnvironment";

export const environment: AgEnvironment = {
    AppUrl: '/api/',
    AppName: 'School Health Information Management System',
    Production: true,
    shortName: 'SHIMS',
} as const;
