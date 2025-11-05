import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <div class="app-shell">
      <header class="app-header">
        <h1>Finance &amp; Zakat Manager</h1>
        <nav>
          <a routerLink="/dashboard">Dashboard</a>
          <a routerLink="/accounts">Accounts</a>
          <a routerLink="/transactions">Transactions</a>
          <a routerLink="/reports">Reports</a>
          <a routerLink="/settings/zakat">Zakat</a>
        </nav>
      </header>
      <main class="app-main">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [
    `
    .app-shell { min-height: 100vh; background: #f5f9fd; color: #102542; }
    .app-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem 2rem; background: #e0f0ff; border-bottom: 1px solid rgba(16,37,66,0.1); }
    .app-header h1 { margin: 0; font-size: 1.5rem; }
    nav a { margin-right: 1rem; color: #102542; text-decoration: none; font-weight: 600; }
    nav a:last-child { margin-right: 0; }
    nav a:hover { text-decoration: underline; }
    .app-main { padding: 2rem; }
    `
  ]
})
export class AppComponent implements OnInit {
  constructor(private readonly auth: AuthService) {}

  ngOnInit(): void {
    if (window.location.hash.includes('access_token')) {
      this.auth.handleCallback(window.location.hash);
      history.replaceState({}, document.title, window.location.pathname);
    }
  }
}
