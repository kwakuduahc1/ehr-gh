import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Routes } from '@angular/router';
import { VitalsHttpService } from './vitals-http.service';
import { RegistrationsHttpService } from '../registrations/registrations-http.service';

export const VitalsRoute: Routes = [
    {
        path: ':id',
        loadComponent: () => import('./components/vitals-list-component/vitals-list-component')
            .then(x => x.VitalsListComponent),
        resolve: {
            details: (id: ActivatedRouteSnapshot) =>
                inject(VitalsHttpService).list(id.params['id'])
        }
    }
];
