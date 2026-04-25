export interface SchemeDrugDTO {
    schemeDrugsID: string;
    drug: string;
    price: number;
    drugCode: string;
    tags: string | null;
    description: string | null;
}

export interface AddSchemeDrugDto {
    schemesID: string;
    drugsID: string;
    price: number;
}
