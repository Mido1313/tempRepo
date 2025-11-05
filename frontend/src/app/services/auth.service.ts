import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _accessToken = signal<string | null>(null);
  private readonly _isAuthenticated = signal<boolean>(false);

  constructor() {
    const token = sessionStorage.getItem('access_token');
    if (token) {
      this._accessToken.set(token);
      this._isAuthenticated.set(true);
    }
  }

  get isAuthenticatedSignal() {
    return this._isAuthenticated.asReadonly();
  }

  isAuthenticated(): boolean {
    return this._isAuthenticated();
  }

  get accessToken() {
    return this._accessToken();
  }

  signIn() {
    const keycloakBase = environment.keycloakUrl ?? 'https://keycloak.local/realms/finance/protocol/openid-connect/auth';
    const clientId = environment.keycloakClientId ?? 'finance-web';
    const redirectUri = encodeURIComponent(window.location.origin + '/dashboard');
    window.location.href = `${keycloakBase}?client_id=${clientId}&response_type=token&redirect_uri=${redirectUri}`;
  }

  handleCallback(hash: string) {
    const params = new URLSearchParams(hash.replace('#', ''));
    const token = params.get('access_token');
    if (token) {
      sessionStorage.setItem('access_token', token);
      this._accessToken.set(token);
      this._isAuthenticated.set(true);
    }
  }

  signOut() {
    sessionStorage.removeItem('access_token');
    this._accessToken.set(null);
    this._isAuthenticated.set(false);
  }
}
