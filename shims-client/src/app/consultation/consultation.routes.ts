import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Routes } from '@angular/router';
import { VitalsHttpService } from '../vitals/vitals-http.service';
import { ConsultingVitalsHttpService } from './http/vitals-http.service';
import { PatientSchemesHttpService } from '../registrations/patient-schemes-http.service';
import { RegistrationsHttpService } from '../registrations/registrations-http.service';

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
        resolve: {
            patient: (param: ActivatedRouteSnapshot) => inject(RegistrationsHttpService).getRegistration(param.paramMap.get('id')!)
        },
        children: [
            {
                path: 'vitals',
                loadComponent: () => import('./components/consult-vitals-component/consult-vitals-component')
                    .then(m => m.ConsultVitalsComponent),
                resolve: {
                    vitals: (param: ActivatedRouteSnapshot) => inject(ConsultingVitalsHttpService).list(param.paramMap.get('id')!)
                }
            },
            {
                path: 'history',
                loadComponent: () => import('./components/consult-history-component/consult-history-component')
                    .then(m => m.ConsultHistoryComponent)
            },
            {
                path: 'investigations',
                loadComponent: () => import('./components/consult-investigations-component/consult-investigations-component')
                    .then(m => m.ConsultInvestigationsComponent)
            },
            {
                path: 'labs',
                loadComponent: () => import('./components/consult-labs-component/consult-labs-component')
                    .then(m => m.ConsultLabsComponent)
            },
            {
                path: 'drugs',
                loadComponent: () => import('./components/consult-drugs-component/consult-drugs-component')
                    .then(m => m.ConsultDrugsComponent)
            },
            {
                path: 'summary',
                loadComponent: () => import('./components/consult-summary-component/consult-summary-component')
                    .then(m => m.ConsultSummaryComponent)
            },
            {
                path: 'services',
                loadComponent: () => import('./components/consult-services-component/consult-services-component')
                    .then(m => m.ConsultServicesComponent)
            },
            {
                path: 'diagnoses',
                loadComponent: () => import('./components/consult-diagnoses-component/consult-diagnoses-component')
                    .then(m => m.ConsultDiagnosesComponent)
            },
            {
                path: 'outcomes',
                loadComponent: () => import('./components/consult-outcomes-component/consult-outcomes-component')
                    .then(m => m.ConsultOutcomesComponent)
            },
            {
                path: '**',
                redirectTo: 'vitals'
            }
        ]
    }
];
