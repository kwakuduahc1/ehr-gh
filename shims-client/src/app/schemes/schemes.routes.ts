import { Routes } from '@angular/router';
import { SchemesListComponent } from './components/schemes-list-component/schemes-list-component';

export const SchemesRoute: Routes = [
    {
        path: '',
        component: SchemesListComponent,
    },
];
