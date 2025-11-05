import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiClientService, AccountDto } from '../services/api-client.service';
import { MoneyInputComponent } from '../components/money-input.component';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, MoneyInputComponent],
  template: `
    <section class="accounts">
      <div class="card">
        <h2>Accounts</h2>
        <p class="muted">Manage cash, bank, and investment accounts included in your zakat calculations.</p>
        <ul>
          <li *ngFor="let account of accounts()">
            <div>
              <strong>{{ account.name }}</strong>
              <span>{{ account.currency }}</span>
            </div>
            <a [routerLink]="['/accounts', account.id]">Details</a>
          </li>
        </ul>
      </div>

      <div class="card">
        <h3>Add account</h3>
        <form (ngSubmit)="submit()">
          <label>Name
            <input type="text" [(ngModel)]="form.name" name="name" required />
          </label>
          <label>Currency
            <input type="text" [(ngModel)]="form.currency" name="currency" maxlength="3" />
          </label>
          <app-money-input [(amount)]="form.openingBalance"></app-money-input>
          <label>
            <input type="checkbox" [(ngModel)]="form.includeInZakat" name="include" /> Include in zakat calculations
          </label>
          <button type="submit">Save</button>
        </form>
      </div>
    </section>
  `,
  styles: [`
    .accounts { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 2rem; }
    ul { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 1rem; }
    li { display: flex; justify-content: space-between; align-items: center; }
    form { display: flex; flex-direction: column; gap: 1rem; }
    input[type="text"] { padding: 0.75rem 1rem; border-radius: 12px; border: 1px solid rgba(16,37,66,0.15); }
    .muted { color: rgba(16,37,66,0.6); }
  `]
})
export class AccountsPageComponent implements OnInit {
  accounts = signal<AccountDto[]>([]);
  form = {
    name: '',
    currency: 'USD',
    openingBalance: 0,
    includeInZakat: true
  };

  constructor(private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh() {
    this.api.getAccounts().subscribe(accounts => this.accounts.set(accounts));
  }

  submit() {
    alert('Account saved (API integration pending).');
  }
}
