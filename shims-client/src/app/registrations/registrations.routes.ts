import { Routes } from '@angular/router';
import { RegistrationsListComponent } from './components/registrations-list-component/registrations-list-component';
import { inject } from '@angular/core';
import { RegistrationsHttpService } from './registrations-http.service';
import { SchemesHttpService } from '../schemes/schemes-http.service';

export const RegistrationsRoute: Routes = [
    {
        path: '',
        component: RegistrationsListComponent,
        resolve: {
            list: () => inject(RegistrationsHttpService).list()
        }
    }
];
