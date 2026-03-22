import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  responseMessage: string = '';
  isSuccess: boolean = false;
  email: string = '';

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.resetPasswordForm = this.fb.group({
      token: ['', Validators.required],
      newPassword: ['', [
        Validators.required,
        Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,20}$/)
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    return newPassword && confirmPassword && newPassword.value === confirmPassword.value 
      ? null : { passwordMismatch: true };
  }

  onSubmit() {
    if (this.resetPasswordForm.valid) {
      const resetData = {
        token: this.resetPasswordForm.value.token,
        newPassword: this.resetPasswordForm.value.newPassword,
        confirmPassword: this.resetPasswordForm.value.confirmPassword
      };

      this.apiService.resetPassword(resetData, resetData.token)
        .subscribe({
          next: (response: any) => {
            this.isSuccess = response.success;
            this.responseMessage = response.message;
            if (response.success) {
              setTimeout(() => {
                this.router.navigate(['/login']);
              }, 2000);
            }
          },
          error: (error) => {
            this.isSuccess = false;
            this.responseMessage = error.error?.message || 'Password reset failed';
          }
        });
    }
  }
}