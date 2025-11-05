import { Component, Input } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-donut-chart',
  standalone: true,
  imports: [NgxChartsModule],
  template: `
    <ngx-charts-pie-chart
      [view]="[300, 300]"
      [results]="data"
      [doughnut]="true"
      [labels]="true"
      [legend]="true"
      [scheme]="{ domain: colors }"
    ></ngx-charts-pie-chart>
  `
})
export class DonutChartComponent {
  @Input() data: { name: string; value: number }[] = [];
  @Input() colors: string[] = ['#4f9fd8', '#6bc4b8', '#f9a620', '#845ef7'];
}
