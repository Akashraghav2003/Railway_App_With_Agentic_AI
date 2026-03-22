import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  responseMessage: string = '';
  loginSuccess: boolean = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      emailOrUserName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    const loginData = this.loginForm.value;

    this.http.post<any>('http://localhost:5063/RailwayTicketBooking/login', loginData).subscribe({
      next: (res) => {
        this.responseMessage = res.message;
        this.loginSuccess = true;

        if (res.data) {
          localStorage.setItem('authToken', res.data);

          try {
            const decodedToken: any = jwtDecode(res.data);
            const userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            if (userRole === 'Admin') {
              this.router.navigate(['/admindashboard']);
            } else if (userRole === 'User') {
              this.router.navigate(['/userdashboard']);
            } else {
              this.responseMessage = 'Unknown user role';
            }
          } catch (error) {
            console.error('Token decoding failed', error);
            this.responseMessage = 'Invalid token received';
          }
        }
      },
      error: (err) => {
        this.responseMessage = err.error?.message || 'Login failed';
        this.loginSuccess = false;
      }
    });
  }
}
