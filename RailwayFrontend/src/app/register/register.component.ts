import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  responseMessage: string = '';
  registerSuccess: boolean = false;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      gender: ['', Validators.required],
      age: [null, [Validators.required, Validators.min(15), Validators.max(100)]],
      address: ['', Validators.required],
      email: ['', [Validators.email]],
      phone: [null, Validators.required],
      userName: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9]{3,20}$')]],
      password: ['', [Validators.required, Validators.pattern('^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{6,20}$')]]
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.registerForm.get(controlName);
    if (control?.hasError('required')) return 'This field is required';
    if (control?.hasError('email')) return 'Please enter a valid email address';
    if (control?.hasError('min') || control?.hasError('max')) return 'Age must be between 15 and 100';
    if (control?.hasError('maxlength')) return 'Name must be less than 50 characters';
    if (control?.hasError('pattern')) {
      if (controlName === 'userName') return 'Username must be 3–20 characters, no spaces or special characters';
      if (controlName === 'password') return 'Password must include uppercase, lowercase, digit, special character (6–20 chars)';
    }
    return '';
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.http.post<any>('http://localhost:5063/RailwayTicketBooking/UserRegister', this.registerForm.value).subscribe({
      next: (res) => {
        this.responseMessage = res.message;
        this.registerSuccess = true;
        this.registerForm.reset();
      },
      error: (err) => {
        this.responseMessage = err.error?.message || 'Registration failed';
        this.registerSuccess = false;
      }
    });
  }
}
