import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiClientService, ZakatHistoryDto } from '../services/api-client.service';

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="card">
      <h2>Zakat settings</h2>
      <p>Use the latest gold or silver nisab to calculate your zakat obligation.</p>
      <div class="actions">
        <button (click)="calculate(true)">Calculate with gold standard</button>
        <button (click)="calculate(false)">Calculate with silver standard</button>
      </div>
      <h3>History</h3>
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Wealth</th>
            <th>Zakat due</th>
            <th>Standard</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let item of history()">
            <td>{{ item.createdUtc | date:'medium' }}</td>
            <td>{{ item.zakatableWealth | currency:item.currency }}</td>
            <td>{{ item.amountDue | currency:item.currency }}</td>
            <td>{{ item.usedGoldStandard ? 'Gold' : 'Silver' }}</td>
          </tr>
        </tbody>
      </table>
    </section>
  `,
  styles: [`
    .actions { display: flex; gap: 1rem; margin-bottom: 1.5rem; }
    table { width: 100%; border-collapse: collapse; }
    th, td { padding: 0.75rem; text-align: left; }
  `]
})
export class ZakatSettingsPageComponent implements OnInit {
  private readonly historySignal = signal<ZakatHistoryDto[]>([]);

  constructor(private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.api.getZakatHistory().subscribe(data => this.historySignal.set(data));
  }

  calculate(useGold: boolean) {
    this.api.calculateZakat(useGold).subscribe(result => {
      this.historySignal.update(history => [result, ...history]);
    });
  }

  history() {
    return this.historySignal();
  }
}
