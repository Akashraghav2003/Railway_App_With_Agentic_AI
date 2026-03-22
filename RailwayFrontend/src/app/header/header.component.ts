import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  currentTime = new Date();
  username = 'User';

  constructor(private router: Router, private userService: UserService, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadUserName();
    setInterval(() => {
      this.currentTime = new Date();
    }, 1000);
  }

  loadUserName(): void {
    const userId = this.getUserIdFromToken();
    if (userId) {
      this.userService.getUserById(userId).subscribe({
        next: (response) => {
          if (response.success) {
            this.username = response.data.name;
          }
        },
        error: () => {
          this.username = 'User';
        }
      });
    }
  }

  private getUserIdFromToken(): number | null {
    const token = localStorage.getItem('authToken');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.UserId);
      } catch (e) {
        return null;
      }
    }
    return null;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login'], { replaceUrl: true });
    window.history.pushState(null, '', '/login');
    window.addEventListener('popstate', () => {
      window.history.pushState(null, '', '/login');
    });
  }
}