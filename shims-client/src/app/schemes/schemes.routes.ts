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
];
