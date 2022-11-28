import { Component, OnInit, Input, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { AlertDialogModel } from '../alert-dialog';
import { AlertSnackModel } from './alert-snack-model';

@Component({
  selector: 'app-alert-snack',
  template: '<div class="base-snack">{{message}}</div>',
  styles: [`
    .base-snack {
      width:100%;
      color: 'white';
      text-align: center;
    }
  `]
})
export class AlertSnackComponent implements OnInit {

  constructor(public dialogRef: MatSnackBarRef<AlertSnackComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: AlertSnackModel) {
    this.message = this.data.message;
  }

  message: string;

  ngOnInit() {
  }

}
