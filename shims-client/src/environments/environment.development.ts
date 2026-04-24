import { AgEnvironment } from "./IEnvironment";

export const environment: AgEnvironment = {
    AppUrl: 'https://localhost:7199/api/',
    AppName: 'Simple Health Information Management Systems',
    Production: false,
    shortName: 'SHIMS',
} as const;
