import { Routes } from '@angular/router';

export const ConsultationRoute: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/consult-list-component/consult-list-component')
            .then(m => m.ConsultListComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./components/patient-consultation-component/patient-consultation-component')
            .then(m => m.PatientConsultationComponent),
        children: [
            {
                path: 'investigations',
                loadComponent: () => import('./components/consult-investigations/consult-investigations')
                    .then(m => m.ConsultInvestigationsComponent)
            },
            {
                path: 'services',
                loadComponent: () => import('./components/consult-services/consult-services')
                    .then(m => m.ConsultServicesComponent)
            },
            {
                path: 'drugs',
                loadComponent: () => import('./components/consult-drugs/consult-drugs')
                    .then(m => m.ConsultDrugsComponent)
            },
            {
                path: 'diagnoses',
                loadComponent: () => import('./components/consult-diagnoses/consult-diagnoses')
                    .then(m => m.ConsultDiagnosesComponent)
            },
            {
                path: 'outcomes',
                loadComponent: () => import('./components/consult-outcomes/consult-outcomes')
                    .then(m => m.ConsultOutcomesComponent)
            }
        ]
    }
];
