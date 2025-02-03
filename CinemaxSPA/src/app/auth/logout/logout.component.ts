import { Component } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-logout',
  imports: [],
  templateUrl: './logout.component.html',
})
export class LogoutComponent {
  constructor(private authenticationService: AuthenticationService){
    this.authenticationService.logout().subscribe();
  }
}
