import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateRangePickerComponent } from '../components/date-range-picker.component';
import { DonutChartComponent } from '../components/donut-chart.component';
import { LineChartComponent } from '../components/line-chart.component';
import { ApiClientService, CategorySpendDto, CashflowReport } from '../services/api-client.service';

@Component({
  standalone: true,
  imports: [CommonModule, DateRangePickerComponent, DonutChartComponent, LineChartComponent],
  template: `
    <section class="card reports">
      <h2>Reports</h2>
      <app-date-range-picker [(from)]="from" [(to)]="to"></app-date-range-picker>
      <div class="report-grid">
        <div class="card">
          <h3>Cashflow</h3>
          <app-line-chart [data]="cashflowSeries()"></app-line-chart>
        </div>
        <div class="card">
          <h3>Category spend</h3>
          <app-donut-chart [data]="categoryData()"></app-donut-chart>
        </div>
      </div>
      <button (click)="refresh()">Refresh</button>
    </section>
  `,
  styles: [`
    .reports { display: flex; flex-direction: column; gap: 2rem; }
    .report-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 2rem; }
  `]
})
export class ReportsPageComponent implements OnInit {
  from = '';
  to = '';
  private readonly cashflow = signal<CashflowReport | null>(null);
  private readonly categorySpend = signal<CategorySpendDto[]>([]);

  constructor(private readonly api: ApiClientService) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh() {
    const params: Record<string, string> = {};
    if (this.from) params['from'] = this.from;
    if (this.to) params['to'] = this.to;
    this.api.getCashflowReport(params).subscribe(data => this.cashflow.set(data));
    this.api.getCategorySpend(params).subscribe(data => this.categorySpend.set(data));
  }

  cashflowSeries() {
    const report = this.cashflow();
    const points = report ? report.points : [];
    return [{ name: 'Cashflow', series: points.map(point => ({ name: point.date, value: point.inflow - point.outflow })) }];
  }

  categoryData() {
    return this.categorySpend().map(item => ({ name: item.category, value: item.amount }));
  }
}
