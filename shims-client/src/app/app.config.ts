import { ApplicationConfig, provideBrowserGlobalErrorListeners, isDevMode, importProvidersFrom, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideServiceWorker } from '@angular/service-worker';
import { ScrollStrategyOptions } from '@angular/cdk/overlay';
import { provideHttpClient, withInterceptors, withFetch, withXsrfConfiguration } from '@angular/common/http';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { JwtModule } from '@auth0/angular-jwt';
import { environment } from '../environments/environment';
import { HttpInterceptorProviders } from './interceptors/InterceptorProviders';
import { MAT_DIALOG_SCROLL_STRATEGY } from '@angular/material/dialog';
import { provideNativeDateAdapter } from '@angular/material/core';

export function tokenGetter() {
  return localStorage.getItem('jwt');
}

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
      useValue: { duration: 10_000, panelClass: ['snackbar-success'] },
    },
    {
      provide: MAT_DIALOG_SCROLL_STRATEGY,
      useFactory: (sto: ScrollStrategyOptions) => sto.reposition,
      deps: [ScrollStrategyOptions],
    },
    provideNativeDateAdapter(),
    provideZonelessChangeDetection(),
    provideBrowserGlobalErrorListeners(),
    importProvidersFrom(
      JwtModule.forRoot({
        config: {
          tokenGetter: tokenGetter,
          allowedDomains: [environment.AppUrl],
        },
      }),
    ),
    provideHttpClient(
      withInterceptors(HttpInterceptorProviders),
      withFetch(),
      withXsrfConfiguration({ cookieName: 'XSRF-TOKEN', headerName: 'X-XSRF-TOKEN' }),
    ),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
    ),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000',
    }),
    provideClientHydration(withEventReplay()),
  ],

};
