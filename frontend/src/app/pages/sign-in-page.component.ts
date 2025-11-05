import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="auth-shell">
      <div class="card">
        <h2>Sign in</h2>
        <p class="lead">Access your family finances and zakat dashboard.</p>
        <form (ngSubmit)="signIn()">
          <label>Email
            <input type="email" [(ngModel)]="email" name="email" required />
          </label>
          <button type="submit">Continue</button>
        </form>
        <div class="social-buttons">
          <button type="button" (click)="signIn()">Sign in with Google</button>
          <button type="button" (click)="signIn()">Sign in with Apple</button>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .auth-shell { display: grid; place-items: center; min-height: calc(100vh - 140px); }
    form { display: flex; flex-direction: column; gap: 1rem; }
    input { padding: 0.75rem 1rem; border-radius: 12px; border: 1px solid rgba(16,37,66,0.1); }
    .social-buttons { display: flex; gap: 1rem; margin-top: 1rem; }
    .lead { color: rgba(16,37,66,0.7); margin-bottom: 1.5rem; }
  `]
})
export class SignInPageComponent {
  email = '';

  constructor(private readonly authService: AuthService) {}

  signIn() {
    this.authService.signIn();
  }
}
