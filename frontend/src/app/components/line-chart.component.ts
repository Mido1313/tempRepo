import { Component, Input } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-line-chart',
  standalone: true,
  imports: [NgxChartsModule],
  template: `
    <ngx-charts-line-chart
      [view]="[400, 300]"
      [scheme]="{ domain: colors }"
      [results]="data"
      [autoScale]="true"
      [timeline]="true"
    ></ngx-charts-line-chart>
  `
})
export class LineChartComponent {
  @Input() data: { name: string; series: { name: string; value: number }[] }[] = [];
  @Input() colors: string[] = ['#4f9fd8', '#f9a620'];
}
