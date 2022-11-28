import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { AlertDialogComponent, AlertDialogModel } from '../components/alert-dialog';
import { AlertSnackComponent } from '../components/alert-snack/alert-snack.component';
import { AlertSnackModel } from '../components/alert-snack/alert-snack-model';
import { ConfirmDialogComponent, ConfirmDialogModel } from '../components/confirm-dialog';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private router: Router,
    private snackBar: MatSnackBar,
    public dialog: MatDialog) { }

  showAlert(message: string, buttonText: string = 'Aceptar') {
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '500px',
      data: new AlertDialogModel('Información', message, buttonText)
    });

    return dialogRef;
  }

  showConfirm(message: string) {
    const dialogData = new ConfirmDialogModel('Información', message);
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    return dialogRef;
  }

  showToast(message: string, duration: number = 2000, verticalPosition: AlertSnackVerticalPosition = AlertSnackVerticalPosition.top) {
    let result = this.snackBar.openFromComponent(AlertSnackComponent, {
      duration: duration,
      data: new AlertSnackModel(message),
      verticalPosition: verticalPosition
    });

    return result;
  }
}

export enum AlertSnackVerticalPosition {
  top = 'top',
  bottom = 'bottom'
}

