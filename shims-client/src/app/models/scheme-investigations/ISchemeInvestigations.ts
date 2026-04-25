export interface SchemeInvestigationDTO {
    schemeInvestigationsID: string;
    investigationsID: string;
    schemesID: string;
    gdrg: string | null;
    price: number;
    isActive: boolean;
    investigation: string | null;
    narration: string | null;
}

export interface AddSchemeInvestigationDto {
    investigationsID: string;
    schemesID: string;
    gdrg: string | null;
    price: number;
    narration: string | null;
}
