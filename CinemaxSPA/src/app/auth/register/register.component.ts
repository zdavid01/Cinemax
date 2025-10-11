import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { IPayPalConfig, NgxPayPalModule, ICreateSubscriptionRequest } from 'ngx-paypal';
import { AuthenticationService, IRegisterRequest, IRegisterResponse } from '../../services/authentication.service';
import { MatError, MatFormFieldModule } from '@angular/material/form-field';
import { errorForFormField, formFieldState } from './register-common';
import { Router } from '@angular/router';

export const stringifyErrors = (errors: Map<string, Array<string>>): string[] => {
  return Object.values(errors).flatMap(errors => [...errors]);
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, MatCardModule, MatInputModule, MatButtonModule, NgxPayPalModule, MatFormFieldModule, MatError],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  submit() {
    if(this.registerForm.get("password")?.value !== this.registerForm.get("confirmPassword")?.value){
      this.registerForm.get("password")?.setErrors({invalid: true, message: "Passwords do not match"})
      return;
    }
    this.errors = new Map();
    this.error = [];

    const registerRequest: IRegisterRequest = {
      username: this.registerForm.value.username,
      firstName: this.registerForm.value.firstName,
      lastName: this.registerForm.value.lastName,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password
    }
    this.authenticationService.register(registerRequest).subscribe((response: IRegisterResponse) => {
      if (response.success) {
        console.log("reguest was successfull");
        this.router.navigate(["/login"]);
      } else if (response.error !== null) {
        this.error = stringifyErrors(response.error);
        this.errors = new Map(Object.entries(response.error));
        this.setErrorsOnForm(this.errors);
        console.log(this.error);
      }
    })
  }
  error: string[] = [];
  registerForm: FormGroup;
  errors: Map<string, Array<string>> = new Map();

  public payPalConfig: IPayPalConfig;

  ngOnInit(): void {
    this.payPalConfig = this.makePaypalConfig();
  }

  private setErrorsOnForm (errors: Map<string, Array<string>>) {
    this.registerForm.get("firstName")?.setErrors(formFieldState("firstName", errors));
    this.registerForm.get("lastName")?.setErrors(formFieldState("lastName", errors));
    this.registerForm.get("username")?.setErrors(formFieldState("username", errors));
    this.registerForm.get("email")?.setErrors(formFieldState("email", errors));
    this.registerForm.get("password")?.setErrors(formFieldState("password", errors));
  }


  makePaypalConfig(): IPayPalConfig {
    const config: IPayPalConfig = {
      clientId: 'ATmo_0gy_93n-0pDTvvxwFTeba38h2lYt-4GPdZDHLLvnwo03ExXIhGLK1JVVoAMF2bJhJcWkDadLJdR',
      onApprove: console.log,
      onCancel: console.log,
      onError: console.log,
      currency: 'RSD',
      vault: 'true',
      createSubscriptionOnClient: (data) => <ICreateSubscriptionRequest>{
        plan_id: "123",
        custom_id: "some id",
        quantity: 1,

      }
    }

    return config;
  }

  constructor(private authenticationService: AuthenticationService, private router: Router) {

    this.payPalConfig = this.makePaypalConfig();
    this.registerForm = new FormGroup({
      firstName: new FormControl(''),
      lastName: new FormControl('', [Validators.required]),
      username: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required]),
    });
  }
}
