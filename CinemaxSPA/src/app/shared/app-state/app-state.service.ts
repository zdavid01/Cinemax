import { Injectable } from '@angular/core';
import { AppState, IAppState } from './app-state';
import { BehaviorSubject, Observable } from 'rxjs';
import { Role } from './role';
import { LocalStorageService } from '../local-storage/local-storage.service';
import { LocalStorageKeys } from '../local-storage/local-storage-keys';
import { L } from '@angular/cdk/keycodes';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  private appState: IAppState = new AppState();
  private appStateSubject: BehaviorSubject<IAppState> = new BehaviorSubject<IAppState>(this.appState);
  private appStateObservable: Observable<IAppState> = this.appStateSubject.asObservable();

  constructor(private localStorageService: LocalStorageService) {
    this.restoreFromLocalStorage();
  }

  public getAppState(): Observable<IAppState> {
    return this.appStateObservable;
  }

  setAccessToken(accessToken: string) {
    this.appState = this.appState.clone();
    this.appState.accessToken = accessToken;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  setRefreshToken(refreshToken: string) {
    this.appState = this.appState.clone();
    this.appState.refreshToken = refreshToken;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  setUsername(username: string) {
    this.appState = this.appState.clone();
    this.appState.username = username;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  setRoles(roles: Role | Role[]) {
    this.appState = this.appState.clone();
    this.appState.roles = roles;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  setEmail(email: string) {
    this.appState = this.appState.clone();
    this.appState.email = email;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }


  setFirstName(firstName: string) {
    this.appState = this.appState.clone();
    this.appState.firstName = firstName;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }


  setLastName(lastName: string) {
    this.appState = this.appState.clone();
    this.appState.lastName = lastName;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }


  setUserId(userId: string) {
    this.appState = this.appState.clone();
    this.appState.userId = userId;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  setIsPremium(isPremium: boolean) {
    this.appState = this.appState.clone();
    this.appState.isPremium = isPremium;
    this.localStorageService.set(LocalStorageKeys.AppState, this.appState);
    this.appStateSubject.next(this.appState);
  }

  public clearAppState(): void {
    this.appState = new AppState();
    this.appStateSubject.next(this.appState);
    this.localStorageService.clear(LocalStorageKeys.AppState);
  }

  private restoreFromLocalStorage(): void {
    const appState: IAppState | null = this.localStorageService.get(LocalStorageKeys.AppState);
    if (appState !== null) {
      this.appState = new AppState(
        appState.accessToken,
        appState.refreshToken,
        appState.username,
        appState.roles,
        appState.email,
        appState.firstName,
        appState.lastName,
        appState.userId,
        appState.isPremium
      );
      this.appStateSubject.next(this.appState);
    }
  }
}

