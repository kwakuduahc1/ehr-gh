import { Routes } from '@angular/router';

export const OpdRoute: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/opd-dashboard/opd-dashboard.component')
            .then(m => m.OpdComponent)
    }
];
