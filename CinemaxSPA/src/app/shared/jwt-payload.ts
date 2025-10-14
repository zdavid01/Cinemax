import { Role } from "./app-state/role";

export enum JwtPayloadKeys {
    Username = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
    Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
    Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
    IsPremium = "IsPremium"
}

export interface IJwtPayload {
    [JwtPayloadKeys.Username]: string;
    [JwtPayloadKeys.Email]: string;
    [JwtPayloadKeys.Role]: Role | Role[];
    [JwtPayloadKeys.IsPremium]?: string;
    exp: number;
    iss: string;
    aud: string;
}
