import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { LogoutComponent } from './auth/logout/logout.component';

export const routes: Routes = [
    {
        path: "login", component: LoginComponent,
    },
    {
        path: "logout", component: LogoutComponent,
    },
    {
        path: "register", component: RegisterComponent
    }
];
