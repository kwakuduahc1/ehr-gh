export interface SchemesDTO {
    schemesID: string;
    schemeName: string;
    coverage: string;
    maxPayable: number;
    recovery: number;
}

export interface AddSchemeDto {
    schemeName: string;
    coverage: string;
    maxPayable: number;
    recovery: number;
}

export interface UpdateSchemeDto {
    schemesID: string;
    schemeName: string;
    coverage: string;
    maxPayable: number;
    recovery: number;
}
