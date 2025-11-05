import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="card onboarding">
      <h2>When did you take possession of your zakatable wealth?</h2>
      <p>Select the date you first met the nisab threshold for your current assets.</p>
      <input type="date" [(ngModel)]="possessionDate" />
      <button (click)="save()">Save date</button>
    </section>
  `,
  styles: [`
    .onboarding { max-width: 480px; margin: 0 auto; display: flex; flex-direction: column; gap: 1rem; }
    input { padding: 0.75rem 1rem; border-radius: 12px; border: 1px solid rgba(16,37,66,0.1); }
  `]
})
export class OnboardingPageComponent {
  possessionDate = '';

  save() {
    alert(`Saved possession date: ${this.possessionDate}`);
  }
}
