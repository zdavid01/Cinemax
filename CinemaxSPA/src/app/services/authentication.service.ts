import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppStateService } from '../shared/app-state/app-state.service';
import { catchError, map, Observable, of, switchMap, take } from 'rxjs';
import { JwtService } from '../shared/jwt/jwt.service';
import { JwtPayloadKeys } from '../shared/jwt-payload';
import { IUserDetails, UserService } from './user.service';
import { IAppState } from '../shared/app-state/app-state';

interface ILoginResponse {
  accessToken: string;
  refreshToken: string;
}

interface IRefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

export interface IRegisterRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  email: string;
}

export interface IRegisterResponse {
  success: boolean,
  error: Map<string, Array<string>>
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(private httpClient: HttpClient, private appStateService: AppStateService, private jwtService: JwtService, private userService: UserService) { }

  refreshToken(): Observable<string | null> {
    return this.appStateService.getAppState().pipe(
      take(1),
      map((appState: IAppState) => {
        const request = { username: appState.username, refreshToken: appState.refreshToken }
        return request;
      }),
      switchMap((request: { username: string | undefined, refreshToken: string | undefined }) => this.httpClient.post<IRefreshTokenResponse>("http://localhost:4000/api/v1/Authentication/Refresh", request)),
      map((response: IRefreshTokenResponse) => {
        this.appStateService.setAccessToken(response.accessToken);
        this.appStateService.setRefreshToken(response.refreshToken);
        return response.accessToken;
      }),
      catchError((err) => {
        console.error(err);
        this.appStateService.clearAppState();
        return of(null);
      }))
  }

  logout(): Observable<Object> {
    return this.appStateService.getAppState().pipe(
      take(1),
      map((appState: IAppState) => {
        const request = { username: appState.username, refreshToken: appState.refreshToken }
        return request;
      }),
      switchMap((request: Object) => {
        return this.httpClient.post("http://localhost:4000/api/v1/Authentication/Logout", request);
      }),
      map(() => {
        this.appStateService.clearAppState();
        return true;
      }),
      catchError((err) => {
        console.error(err);
        return of(false);
      })
    )
  }

  login(username: string, password: string) {
    return this.httpClient.post('http://localhost:4000/api/v1/Authentication/Login', { username, password }).pipe(
      switchMap((response) => {
        const loginResponse = response as ILoginResponse;
        this.appStateService.setAccessToken(loginResponse.accessToken);
        this.appStateService.setRefreshToken(loginResponse.refreshToken);

        const payload = this.jwtService.parsePayload(loginResponse.accessToken);
        this.appStateService.setUsername(payload[JwtPayloadKeys.Username])
        this.appStateService.setRoles(payload[JwtPayloadKeys.Role]);
        this.appStateService.setEmail(payload[JwtPayloadKeys.Email])

        return this.userService.getUserDetails(payload[JwtPayloadKeys.Username]);
      }), map((userDetails: IUserDetails) => {
        this.appStateService.setFirstName(userDetails.firstName);
        this.appStateService.setLastName(userDetails.lastName);
        this.appStateService.setUserId(userDetails.id);
        return true;

      }), catchError((err) => {
        console.error('Error during login ', err);
        this.appStateService.clearAppState();
        return of(false);
      })
    );
  }

  register(registerRequest: IRegisterRequest): Observable<IRegisterResponse> {
    return this.httpClient.post('http://localhost:4000/api/v1/Authentication/RegisterBuyer', registerRequest).pipe(
      switchMap(() => {
        return of({
          error: new Map<string, Array<string>>(),
          success: true
        })
      }),
      catchError((err) => {
        console.log('Error during register', err);
        return of({ success: false, error: err.error.errors || err.error });
      })
    )
  }
}
