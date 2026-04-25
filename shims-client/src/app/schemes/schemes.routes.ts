import { Routes } from '@angular/router';
import { SchemesListComponent } from './components/schemes-list-component/schemes-list-component';
import { inject } from '@angular/core';
import { SchemesHttpService } from './schemes-http.service';

export const SchemesRoute: Routes = [
    {
        path: '',
        component: SchemesListComponent,
        resolve: {
            schemes: () => inject(SchemesHttpService).getSchemes()
        }
    },
    {
        path: ':id/drugs',
        loadComponent: () => import('./components/scheme-drug-component/scheme-drug.component')
            .then(m => m.SchemeDrugComponent)
    },
    {
        path: ':id/services',
        loadComponent: () => import('./components/scheme-services-component/scheme-services.component')
            .then(m => m.SchemeServicesComponent)
    }
];
