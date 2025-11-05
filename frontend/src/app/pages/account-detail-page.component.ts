import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ApiClientService, TransactionDto } from '../services/api-client.service';
import { DonutChartComponent } from '../components/donut-chart.component';
import { TagListComponent } from '../components/tag-list.component';

@Component({
  standalone: true,
  imports: [CommonModule, DonutChartComponent, TagListComponent],
  template: `
    <section class="card">
      <h2>Account details</h2>
      <div class="metrics">
        <div>
          <h3>Last 7 days</h3>
          <p>{{ lastWeekTotal() | number:'1.2-2' }}</p>
        </div>
        <div>
          <h3>Last 30 days</h3>
          <p>{{ lastMonthTotal() | number:'1.2-2' }}</p>
        </div>
      </div>

      <div class="chart">
        <app-donut-chart [data]="categoryChart()"></app-donut-chart>
      </div>

      <div class="activity">
        <h3>Activity</h3>
        <ul>
          <li *ngFor="let tx of transactions()">
            <div>
              <strong>{{ tx.amount | number:'1.2-2' }} {{ tx.currency }}</strong>
              <span>{{ tx.bookedOn }}</span>
            </div>
            <app-tag-list [tags]="tx.tags"></app-tag-list>
          </li>
        </ul>
      </div>
    </section>
  `,
  styles: [`
    .metrics { display: flex; gap: 2rem; }
    .metrics div { background: rgba(79,159,216,0.1); padding: 1rem; border-radius: 12px; }
    .activity ul { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 1rem; }
    .activity li { display: flex; justify-content: space-between; align-items: center; }
  `]
})
export class AccountDetailPageComponent implements OnInit {
  private readonly accountId = signal<string>('');
  private readonly transactionsSignal = signal<TransactionDto[]>([]);

  constructor(private readonly route: ActivatedRoute, private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.accountId.set(id);
        this.api.getTransactions({ accountId: id }).subscribe(data => this.transactionsSignal.set(data));
      }
    });
  }

  lastWeekTotal(): number {
    const cutoff = new Date();
    cutoff.setDate(cutoff.getDate() - 7);
    return this.transactionsSignal().filter(tx => new Date(tx.bookedOn) >= cutoff).reduce((sum, tx) => sum + tx.amount, 0);
  }

  lastMonthTotal(): number {
    const cutoff = new Date();
    cutoff.setDate(cutoff.getDate() - 30);
    return this.transactionsSignal().filter(tx => new Date(tx.bookedOn) >= cutoff).reduce((sum, tx) => sum + tx.amount, 0);
  }

  categoryChart(): { name: string; value: number }[] {
    return this.transactionsSignal().reduce((acc, tx) => {
      const key = tx.amount < 0 ? 'Outflow' : 'Inflow';
      const existing = acc.find(item => item.name === key);
      if (existing) {
        existing.value += Math.abs(tx.amount);
      } else {
        acc.push({ name: key, value: Math.abs(tx.amount) });
      }
      return acc;
    }, [] as { name: string; value: number }[]);
  }

  transactions(): TransactionDto[] {
    return this.transactionsSignal();
  }
}
