import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-date-range-picker',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="date-range">
      <label>
        From
        <input type="date" [value]="from" (change)="fromChange.emit(($event.target as HTMLInputElement).value)" />
      </label>
      <label>
        To
        <input type="date" [value]="to" (change)="toChange.emit(($event.target as HTMLInputElement).value)" />
      </label>
    </div>
  `,
  styles: [`
    .date-range { display: flex; gap: 1rem; }
    label { display: flex; flex-direction: column; font-weight: 600; }
    input { padding: 0.5rem 0.75rem; border-radius: 10px; border: 1px solid rgba(16,37,66,0.15); }
  `]
})
export class DateRangePickerComponent {
  @Input() from = '';
  @Input() to = '';
  @Output() fromChange = new EventEmitter<string>();
  @Output() toChange = new EventEmitter<string>();
}
