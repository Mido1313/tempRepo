import { Routes } from '@angular/router';
import { SignInPageComponent } from './pages/sign-in-page.component';
import { OnboardingPageComponent } from './pages/onboarding-page.component';
import { DashboardPageComponent } from './pages/dashboard-page.component';
import { AccountsPageComponent } from './pages/accounts-page.component';
import { AccountDetailPageComponent } from './pages/account-detail-page.component';
import { TransactionsPageComponent } from './pages/transactions-page.component';
import { ReportsPageComponent } from './pages/reports-page.component';
import { ZakatSettingsPageComponent } from './pages/zakat-settings-page.component';
import { authGuard } from './services/auth.guard';

export const routes: Routes = [
  { path: 'signin', component: SignInPageComponent },
  { path: 'onboarding/possession-date', component: OnboardingPageComponent, canActivate: [authGuard] },
  { path: 'dashboard', component: DashboardPageComponent, canActivate: [authGuard] },
  { path: 'accounts', component: AccountsPageComponent, canActivate: [authGuard] },
  { path: 'accounts/:id', component: AccountDetailPageComponent, canActivate: [authGuard] },
  { path: 'transactions', component: TransactionsPageComponent, canActivate: [authGuard] },
  { path: 'reports', component: ReportsPageComponent, canActivate: [authGuard] },
  { path: 'settings/zakat', component: ZakatSettingsPageComponent, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: '**', redirectTo: 'dashboard' }
];
