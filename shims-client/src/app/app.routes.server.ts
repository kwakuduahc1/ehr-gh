import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'register',
    renderMode: RenderMode.Server
  },
  {
    path: 'schemes',
    renderMode: RenderMode.Client
  },
  {
    path: 'registrations',
    renderMode: RenderMode.Client
  },
  {
    path: 'vitals/:id',
    renderMode: RenderMode.Client
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
