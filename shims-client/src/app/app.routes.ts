import { Routes } from '@angular/router';
import { SchemesRoute } from './schemes/schemes.routes';
import { OpdHttpService } from './opd/opd-http.service';
import { inject } from '@angular/core';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        loadComponent: () => import('../app/components/home/home.component')
            .then(x => x.HomeComponent)
    },
    {
        path: 'login',
        loadComponent: () => import('../app/components/login/login.component')
            .then(x => x.LoginComponent)
    },
    {
        path: 'register',
        loadComponent: () => import('../app/components/register/register.component')
            .then(x => x.RegisterComponent)
    },
    {
        path: 'schemes',
        loadChildren: () => import('../app/schemes/schemes.routes')
            .then(x => x.SchemesRoute)
    },
    {
        path: 'registrations',
        loadChildren: () => import('../app/registrations/registrations.routes')
            .then(x => x.RegistrationsRoute)
    },
    {
        path: 'vitals',
        loadChildren: () => import('../app/vitals/vitals.routes')
            .then(x => x.VitalsRoute)
    },
    {
        path: 'opd',
        loadChildren: () => import('../app/opd/opd.routes')
            .then(x => x.OpdRoute),
        resolve: {
            patients: () => inject(OpdHttpService).list()
        }
    }

];
