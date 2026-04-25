export interface SchemeServiceDTO {
    schemeServicesID: string;
    servicesID: string;
    price: number;
    tiers: string[];
    gdrg: string;
    narration: string;
    service: string;
    serviceGroup: string;
}

export interface AddSchemeServiceDto {
    schemesID: string;
    servicesID: string;
    gdrg: string;
    narration: string | null;
    allowedTiers: string[];
    price: number;
}
