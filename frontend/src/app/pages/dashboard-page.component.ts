import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiClientService, AccountDto, TransactionDto } from '../services/api-client.service';
import { DonutChartComponent } from '../components/donut-chart.component';
import { LineChartComponent } from '../components/line-chart.component';
import { TagListComponent } from '../components/tag-list.component';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, DonutChartComponent, LineChartComponent, TagListComponent, DatePipe],
  template: `
    <section class="dashboard">
      <div class="card summary-card">
        <h2>Summary</h2>
        <div class="summary-grid">
          <div>
            <h3>Total income</h3>
            <p>{{ summary()?.totalIncome | currency:'USD':'symbol-narrow' }}</p>
          </div>
          <div>
            <h3>Total expense</h3>
            <p>{{ summary()?.totalExpense | currency:'USD':'symbol-narrow' }}</p>
          </div>
          <div>
            <h3>Net cashflow</h3>
            <p>{{ summary()?.netCashFlow | currency:'USD':'symbol-narrow' }}</p>
          </div>
        </div>
      </div>

      <div class="card chart-card">
        <h3>Spending by category</h3>
        <app-donut-chart [data]="categoryBreakdown()"></app-donut-chart>
      </div>

      <div class="card chart-card">
        <h3>Cashflow trend</h3>
        <app-line-chart [data]="cashflowSeries()"></app-line-chart>
      </div>

      <div class="card account-card">
        <h3>Accounts</h3>
        <ul>
          <li *ngFor="let account of accounts()">
            <div>
              <strong>{{ account.name }}</strong>
              <span>{{ account.currency }}</span>
            </div>
            <a [routerLink]="['/accounts', account.id]">View</a>
          </li>
        </ul>
      </div>

      <div class="card transactions-card">
        <h3>Recent transactions</h3>
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Amount</th>
              <th>Note</th>
              <th>Tags</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let tx of recentTransactions()">
              <td>{{ tx.bookedOn | date }}</td>
              <td [class.negative]="tx.amount < 0">{{ tx.amount | number:'1.2-2' }} {{ tx.currency }}</td>
              <td>{{ tx.note || '—' }}</td>
              <td><app-tag-list [tags]="tx.tags"></app-tag-list></td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  `,
  styles: [`
    .dashboard { display: grid; gap: 2rem; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); }
    .summary-card { grid-column: 1 / -1; }
    .summary-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1.5rem; }
    .summary-grid div { background: rgba(79,159,216,0.1); padding: 1rem; border-radius: 12px; }
    .summary-grid h3 { margin-bottom: 0.5rem; font-size: 0.95rem; text-transform: uppercase; letter-spacing: 0.08em; }
    .summary-grid p { margin: 0; font-size: 1.5rem; font-weight: 700; }
    .account-card ul { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 1rem; }
    .account-card li { display: flex; justify-content: space-between; align-items: center; }
    table { width: 100%; border-collapse: collapse; }
    th, td { padding: 0.75rem; text-align: left; }
    tbody tr:nth-child(every) { background: rgba(0,0,0,0.02); }
    .negative { color: #c92a2a; }
  `]
})
export class DashboardPageComponent implements OnInit {
  private readonly summaryState = signal<{ totalIncome: number; totalExpense: number; netCashFlow: number; generatedUtc: string } | null>(null);
  private readonly accountsState = signal<AccountDto[]>([]);
  private readonly transactionsState = signal<TransactionDto[]>([]);

  constructor(private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.api.getDashboardSummary().subscribe(data => this.summaryState.set(data));
    this.api.getAccounts().subscribe(data => this.accountsState.set(data));
    this.api.getTransactions({ pageSize: 5 }).subscribe(data => this.transactionsState.set(data));
  }

  categoryBreakdown(): { name: string; value: number }[] {
    return this.transactionsState().reduce((acc, tx) => {
      const key = tx.amount < 0 ? 'Expenses' : 'Income';
      const existing = acc.find(item => item.name === key);
      if (existing) {
        existing.value += Math.abs(tx.amount);
      } else {
        acc.push({ name: key, value: Math.abs(tx.amount) });
      }
      return acc;
    }, [] as { name: string; value: number }[]);
  }

  cashflowSeries(): { name: string; series: { name: string; value: number }[] }[] {
    const byDate = new Map<string, number>();
    for (const tx of this.transactionsState()) {
      const current = byDate.get(tx.bookedOn) ?? 0;
      byDate.set(tx.bookedOn, current + tx.amount);
    }

    const series = Array.from(byDate.entries()).map(([date, value]) => ({ name: date, value }));
    return [{ name: 'Cashflow', series }];
  }

  recentTransactions(): TransactionDto[] {
    return this.transactionsState();
  }

  summary(): { totalIncome: number; totalExpense: number; netCashFlow: number; generatedUtc: string } | null {
    return this.summaryState();
  }

  accounts(): AccountDto[] {
    return this.accountsState();
  }
}
