import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tag-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ul class="tag-list">
      <li *ngFor="let tag of tags">{{ tag }}</li>
    </ul>
  `,
  styles: [`
    .tag-list { display: flex; flex-wrap: wrap; gap: 0.5rem; list-style: none; padding: 0; margin: 0; }
    .tag-list li { background: rgba(79, 159, 216, 0.15); color: #0f3b68; padding: 0.25rem 0.75rem; border-radius: 999px; font-size: 0.85rem; font-weight: 600; }
  `]
})
export class TagListComponent {
  @Input() tags: string[] = [];
}
