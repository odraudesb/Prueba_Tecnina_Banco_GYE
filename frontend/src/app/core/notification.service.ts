import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NotificationData, NotificationDialog } from '../shared/notification-dialog';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly dialog = inject(MatDialog);

  show(title: string, message: string, type: NotificationData['type'] = 'success'): void {
    this.dialog.closeAll();
    this.dialog.open(NotificationDialog, {
      data: { title, message, type },
      width: '420px',
      maxWidth: 'calc(100vw - 32px)',
      ariaLabel: title,
    });
  }
}
