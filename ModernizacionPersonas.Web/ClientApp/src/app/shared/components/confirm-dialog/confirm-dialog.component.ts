import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConfirmDialogModel } from './confirm-dialog-model';

@Component({
  selector: 'app-confirm-dialog',
  template: `<h1 mat-dialog-title>{{title}}</h1>
            <div mat-dialog-content>
              <p [innerHtml]="message"></p>
            </div>
            <div mat-dialog-actions class="float-right">
              <button mat-raised-button (click)="onDismiss()">Cancelar</button>
              <button mat-raised-button color="warn" (click)="onConfirm()">Aceptar</button>
            </div>`
})

export class ConfirmDialogComponent {
  title: string;
  message: string;

  constructor(public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogModel) {
    // Update view with given values
    this.title = data.title;
    this.message = data.message;
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}
