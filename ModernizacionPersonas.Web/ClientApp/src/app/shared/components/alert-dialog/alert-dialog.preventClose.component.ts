import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AlertDialogModel } from './alert-dialog-model';

@Component({
  selector: 'app-alert-dialog-prevent-close',
  template: `<div>
                <h1 mat-dialog-title>{{title}}</h1>
             </div>
             <div mat-dialog-content>
                <p [innerHtml]="message"></p>
             </div>`
})
export class AlertDialogPreventCloseComponent {

  title: string;
  message: string;

  constructor(public dialogRef: MatDialogRef<AlertDialogPreventCloseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AlertDialogModel) {
    // Update view with given values
    this.title = this.data.title;
    this.message = this.data.message;
  }

}
