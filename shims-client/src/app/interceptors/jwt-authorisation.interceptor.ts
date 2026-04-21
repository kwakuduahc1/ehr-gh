import { HttpInterceptorFn, HttpXsrfTokenExtractor } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize, tap } from 'rxjs';
import { ActivityProvider } from '../providers/ActivityProvider';
import { BsHandler } from '../models/bsHandler';
import { StatusProvider } from '../providers/StatusProvider';

// export const XSRFInterceptorProvider: HttpInterceptorFn = (req, next) => {
//   let token = inject(HttpXsrfTokenExtractor).getToken();
//   const headerName = 'X-XSRF-TOKEN';
//   if (token !== null && !req.headers.has(headerName)) {
//     return next(req.clone({
//       headers: req.headers.append(headerName, token)
//     }));
//   }
//   return next(req);
// };

export const authorizationInterceptorProvider: HttpInterceptorFn = (req, next) => {
  return next(req
    .clone({
      withCredentials: true,
      headers: req.headers.append('Authorization', inject(StatusProvider).getHeader())
    }))
}

export const errorHandlerInterceptorProvider: HttpInterceptorFn = (req, next) => {
  let act = inject(ActivityProvider);
  let hand = inject(BsHandler);
  act.beginProc();
  return next(req).pipe(
    tap(() => act.beginProc),
    tap({
      error: (err) => hand.onError(err),
      complete: () => act.endProc()
    }),
    finalize(() => act.endProc()),
  )
};
