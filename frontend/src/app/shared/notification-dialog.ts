import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface NotificationData {
  title: string;
  message: string;
  type: 'success' | 'error' | 'info';
}

@Component({
  selector: 'app-notification-dialog',
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <div class="dialog-status" [class]="data.type" aria-hidden="true">
      {{ data.type === 'success' ? '✓' : data.type === 'error' ? '!' : 'i' }}
    </div>
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content><p>{{ data.message }}</p></mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-flat-button mat-dialog-close cdkFocusInitial>Entendido</button>
    </mat-dialog-actions>
  `,
  styles: `
    :host { display: block; padding: 24px 12px 12px; }
    .dialog-status { display: grid; place-items: center; margin: 0 24px 8px;
      width: 44px; height: 44px; border-radius: 50%; font-size: 24px; font-weight: 700; }
    .success { color: #136944; background: #e2f4e9; }
    .error { color: #a82e31; background: #ffe9e8; }
    .info { color: #3156ad; background: #eaf0ff; }
    p { margin-top: 0; line-height: 1.6; }
  `,
})
export class NotificationDialog {
  readonly data = inject<NotificationData>(MAT_DIALOG_DATA);
}
