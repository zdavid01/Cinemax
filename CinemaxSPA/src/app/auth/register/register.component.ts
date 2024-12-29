import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { IPayPalConfig, NgxPayPalModule, ICreateSubscriptionRequest } from 'ngx-paypal';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, MatCardModule, MatInputModule, MatButtonModule, NgxPayPalModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  submit() { }
  error = "";
  loginForm: FormGroup;

  public payPalConfig: IPayPalConfig;

  ngOnInit(): void {
    this.payPalConfig = this.makePaypalConfig();
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

  constructor() {
    this.payPalConfig = this.makePaypalConfig();
    this.loginForm = new FormGroup({
      firstName: new FormControl(''),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required]),
    });
  }
}
