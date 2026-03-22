import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  responseMessage: string = '';
  isSuccess: boolean = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.forgotPasswordForm.valid) {
      const email = this.forgotPasswordForm.value.email;
      
      this.apiService.forgotPassword(email)
        .subscribe({
          next: (response: any) => {
            this.isSuccess = response.success;
            this.responseMessage = response.message;
            if (response.success) {
              setTimeout(() => {
                this.router.navigate(['/reset-password'], { queryParams: { email } });
              }, 2000);
            }
          },
          error: (error) => {
            this.isSuccess = false;
            this.responseMessage = 'Network error occurred';
          }
        });
    }
  }
}