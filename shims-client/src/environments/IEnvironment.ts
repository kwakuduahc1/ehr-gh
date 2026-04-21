interface IEnvironment {
    Production: boolean;
    AppUrl: string;
    AppName: string;
    shortName: string;
}

export type AgEnvironment = IEnvironment;
