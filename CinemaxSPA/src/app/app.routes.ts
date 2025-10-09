import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { LogoutComponent } from './auth/logout/logout.component';
import { ChatComponent } from './chat/chat.component';
import { PrivateSessionComponent } from './private-session/private-session.component';
import { PrivateSessionsList } from './private-session/private-session-list.component';
import { BasketComponent } from './basket/basket.component';
import { PaymentListComponent } from './payment/payment-list.component';
import { PaymentCreateComponent } from './payment/payment-create.component';
import { PayPalPaymentComponent } from './paypal/paypal-payment.component';
import { CatalogComponent } from './catalog/catalog.component';

export const routes: Routes = [
    {
        path: "login", component: LoginComponent,
    },
    {
        path: "logout", component: LogoutComponent,
    },
    {
        path: "register", component: RegisterComponent
    },
    {
        path: "session/:sessionId", component: PrivateSessionComponent,
    },
    {
        path: "sessions", component: PrivateSessionsList
    },
    {
        path: "basket", component: BasketComponent,
    },
    {
        path: 'catalog', component: CatalogComponent

    }
    ,
    { path: "payments", component: PaymentListComponent },
    { path: "payments/create", component: PaymentCreateComponent },
    { path: "paypal", component: PayPalPaymentComponent }
];
