import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap, take } from 'rxjs';
import { AppStateService } from '../shared/app-state/app-state.service';

export interface IUserDetails {
  firstName: string;
  lastName: string;
  id: string;
  isPremium: boolean
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient, private appStateService: AppStateService) { }

  getUserDetails(username: string): Observable<IUserDetails> {
    return this.httpClient.get<IUserDetails>(`http://localhost:4000/api/v1/User/users/${username}`);
  }

  upgradeToPremium(username: string): Observable<{ message: string; isPremium: boolean }> {
    return this.httpClient.post<{ message: string; isPremium: boolean }>(
      'http://localhost:4000/api/v1/User/upgrade-to-premium',
      { username }
    );
  }
}
