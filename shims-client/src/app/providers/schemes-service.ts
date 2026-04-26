import { httpResource } from '@angular/common/http';
import { computed, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { SchemesDTO } from '../models/ISchemes';

@Injectable({ providedIn: 'root' })
export class SchemesService {
    private _schemes = httpResource<SchemesDTO[]>(() => `${environment.AppUrl}Schemes`, {
        parse: (x) => x as SchemesDTO[],
        defaultValue: [],
    }).asReadonly();
    schemes = computed(() => this._schemes.value());
    feePaying = computed(() => this.schemes().filter(x => x.schemeName === 'Fee Paying')[0]);
    insurances = computed(() => this.schemes().filter(x => !x.schemeName.includes('Fee')));

}
