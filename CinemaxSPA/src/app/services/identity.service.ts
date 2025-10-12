import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class IdentityService {
  private baseUrl = 'http://localhost:4000/api/v1/User';

  constructor(private http: HttpClient) {}

  isAdmin(username: string): Observable<boolean> {
    return this.http.get<{ isAdmin: boolean }>(`${this.baseUrl}/${username}/isAdmin`)
      .pipe(map(res => res.isAdmin));
  }
}
