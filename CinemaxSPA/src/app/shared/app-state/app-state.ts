import { Role } from "./role";

export interface IAppState {
    accessToken?: string;
    refreshToken?: string;
    username?: string;
    email?: string;
    roles?: Role | Role[];

    firstName?: string;
    lastName?: string;
    userId?: string;
    isPremium?: boolean;
    clone(): IAppState
}

export class AppState implements IAppState {
    public accessToken?: string | undefined;
    public refreshToken?: string | undefined;
    public username?: string | undefined;
    public roles?: Role | Role[] | undefined;
    public email?: string;
    public firstName?: string | undefined;
    public lastName?: string | undefined;
    public userId?: string | undefined;
    public isPremium?: boolean | undefined

    public constructor();
    public constructor(accessToken?: string, refreshToken?: string, username?: string, roles?: Role | Role[], email?: string, firstName?: string, lastName?: string, userId?: string);

    public constructor(...args: any[]) {
        if (args.length === 0) {
            return;
        }
        this.accessToken = args[0];
        this.refreshToken = args[1]
        this.username = args[2];
        this.roles = args[3];
        this.email = args[4];
        this.firstName = args[5];
        this.lastName = args[6];
        this.userId = args[7];
    }

    public hasRole(role: Role): boolean {
        if (!this.roles) {
            return false;
        }

        if (typeof this.roles === 'string') {
            return this.roles === role;
        }

        return this.roles.some(rl => rl === role);
    }

    public clone(): IAppState {
        const newAppState = new AppState();
        Object.assign(newAppState, this);
        return newAppState;
    }
}
