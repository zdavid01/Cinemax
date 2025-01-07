import { HttpErrorResponse, HttpEvent, HttpHandler, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { AppStateService } from '../app-state/app-state.service';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, map, Observable, switchMap, take, throwError } from 'rxjs';
import { IAppState } from '../app-state/app-state';
import { AuthenticationService } from '../../services/authentication.service';

let globalAccessToken: string | undefined;

let isRefreshing: boolean = false;
let refreshedAccessTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);


export const authenticationInterceptor: HttpInterceptorFn = (req, next) => {
  const whitelistUrls: string[] = [
    'api/v1/Authentication/Login'
  ];

  inject(AppStateService).getAppState().pipe(take(1)).subscribe((appState: IAppState) => {
    globalAccessToken = appState.accessToken;
  });

  const authService = inject(AuthenticationService);

  if (globalAccessToken) {
    req = addToken(req, globalAccessToken);
  }
  return next(req).pipe(catchError((err) => {
    if (err instanceof HttpErrorResponse && err.status === 401) {
      return handle401Error(req, next, authService);
    }
    return throwError(() => err);
  }));
};

const handle401Error = (request: HttpRequest<unknown>, next: HttpHandlerFn, authService: AuthenticationService): Observable<HttpEvent<unknown>> => {
  if(!isRefreshing) {
    isRefreshing = true;
    refreshedAccessTokenSubject.next(null);

    return authService.refreshToken().pipe(
      switchMap((accessToken: string | null) => {
        if(accessToken === null) {
          return throwError(() => new Error("Refresh token flow failed"));
        }

        isRefreshing = false;
        refreshedAccessTokenSubject.next(accessToken);
        return next(addToken(request, accessToken));
      })
    )
  }
  return refreshedAccessTokenSubject.pipe(
    filter((token: string | null) => token !== null),
    take(1),
    switchMap((accessToken: string | null) => next(addToken(request, accessToken as string)))
  )
}

const addToken = (request: HttpRequest<unknown>, accessToken: string): HttpRequest<unknown> => {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  })
}
