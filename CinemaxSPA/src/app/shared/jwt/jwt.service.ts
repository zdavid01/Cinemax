import { Injectable } from '@angular/core';
import { IJwtPayload } from '../jwt-payload';

@Injectable({
  providedIn: 'root'
})
export class JwtService {
  constructor() { }

  public parsePayload(jwtString: string): IJwtPayload {
    const jswStringParts: string[] = jwtString.split('.');

    return JSON.parse(atob(jswStringParts[1]))
  }
}
