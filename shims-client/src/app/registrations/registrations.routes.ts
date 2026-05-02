import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { RegistrationsHttpService } from './registrations-http.service';

export const RegistrationsRoute: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/registrations-list-component/registrations-list-component').then(m => m.RegistrationsListComponent),
        resolve: {
            list: () => inject(RegistrationsHttpService).list()
        },
        title: 'Patient Registrations'
    },
    {
        path: 'search',
        loadComponent: () => import('./components/search-component/search-component').then(m => m.SearchComponent)
    },
    {
        path: 'detail/:id',
        loadComponent: () => import('./components/insurance-detail-component/insurance-detail-component').then(m => m.InsuranceDetailComponent)
    }
];
