import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiClientService, TransactionDto } from '../services/api-client.service';
import { DateRangePickerComponent } from '../components/date-range-picker.component';
import { TagListComponent } from '../components/tag-list.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, DateRangePickerComponent, TagListComponent],
  template: `
    <section class="card">
      <h2>Transactions</h2>
      <div class="filters">
        <input type="search" [(ngModel)]="query" placeholder="Search notes or tags" />
        <app-date-range-picker [(from)]="from" [(to)]="to"></app-date-range-picker>
        <button (click)="load()">Apply filters</button>
      </div>
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
          <tr *ngFor="let tx of transactions()">
            <td>{{ tx.bookedOn }}</td>
            <td [class.negative]="tx.amount < 0">{{ tx.amount | number:'1.2-2' }} {{ tx.currency }}</td>
            <td>{{ tx.note || '—' }}</td>
            <td><app-tag-list [tags]="tx.tags"></app-tag-list></td>
          </tr>
        </tbody>
      </table>
    </section>
  `,
  styles: [`
    .filters { display: flex; gap: 1rem; align-items: flex-end; margin-bottom: 1.5rem; flex-wrap: wrap; }
    input[type="search"] { flex: 1 1 240px; padding: 0.75rem 1rem; border-radius: 12px; border: 1px solid rgba(16,37,66,0.15); }
    table { width: 100%; border-collapse: collapse; }
    th, td { padding: 0.75rem; text-align: left; }
    .negative { color: #c92a2a; }
  `]
})
export class TransactionsPageComponent implements OnInit {
  transactions = signal<TransactionDto[]>([]);
  query = '';
  from = '';
  to = '';

  constructor(private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    const params: Record<string, string> = {};
    if (this.query) {
      params['q'] = this.query;
    }
    if (this.from) {
      params['from'] = this.from;
    }
    if (this.to) {
      params['to'] = this.to;
    }

    this.api.getTransactions(params).subscribe(data => this.transactions.set(data));
  }
}
