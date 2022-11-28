import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AlertDialogModel } from './alert-dialog-model';

@Component({
  selector: 'app-alert-dialog',
  template: `<div>
                <h1 mat-dialog-title>{{title}}</h1>
             </div>
             <div mat-dialog-content>
                <p [innerHtml]="message"></p>
             </div>
             <div mat-dialog-actions class="float-right">
                <button mat-raised-button color="warn" (click)="onDismiss()">{{actionText}}</button>
             </div>`
})
export class AlertDialogComponent {

  title: string;
  message: string;
  actionText: string = 'Cerrar';

  constructor(public dialogRef: MatDialogRef<AlertDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AlertDialogModel) {
    // Update view with given values
    this.title = this.data.title;
    this.message = this.data.message;
    this.actionText = this.data.actionText || 'Cerrar';
  }

  onDismiss(): void {
    this.dialogRef.close(true);
  }
}
