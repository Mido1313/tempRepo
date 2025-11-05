import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-money-input',
  standalone: true,
  imports: [CommonModule],
  template: `
    <label class="money-input">
      <span>{{ label }}</span>
      <div>
        <input type="number" [value]="amount" (input)="onAmountChange($event)" step="0.01" />
        <span class="currency">{{ currency }}</span>
      </div>
    </label>
  `,
  styles: [`
    .money-input { display: flex; flex-direction: column; gap: 0.5rem; font-weight: 600; }
    .money-input input { padding: 0.75rem 1rem; border-radius: 12px; border: 1px solid rgba(16,37,66,0.15); font-size: 1rem; }
    .currency { margin-left: 0.5rem; font-weight: 700; }
  `]
})
export class MoneyInputComponent {
  @Input() label = 'Amount';
  @Input() amount = 0;
  @Input() currency = 'USD';
  @Output() amountChange = new EventEmitter<number>();

  onAmountChange(event: Event) {
    const value = Number((event.target as HTMLInputElement).value);
    this.amountChange.emit(value);
  }
}
