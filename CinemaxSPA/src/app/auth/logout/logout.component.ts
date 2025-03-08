import { Component } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  imports: [],
  templateUrl: './logout.component.html',
})
export class LogoutComponent {
  constructor(private authenticationService: AuthenticationService, private routerService: Router){
    this.authenticationService.logout().subscribe(() => {
      this.routerService.navigate(['/login'])
    });
  }
}
