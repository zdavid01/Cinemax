import { Component } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCardModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  onLoginSubmit() {
    if (this.loginForm.invalid) {
      return;
    }
    this.error = "";
    this.authService.login(this.loginForm.value.username, this.loginForm.value.password).subscribe((successful: boolean) => {
      if (successful) {
        this.routerService.navigate(['/'])
      } else {
        this.error = "Login failed";
      }
    });
  }
  error = "";
  loginForm: FormGroup;

  constructor(private authService: AuthenticationService, private routerService: Router) {
    this.loginForm = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
    });
  }
}
