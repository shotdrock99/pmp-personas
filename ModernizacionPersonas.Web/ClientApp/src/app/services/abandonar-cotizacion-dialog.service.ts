import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ConfirmDialogComponent, ConfirmDialogModel } from '../shared/components/confirm-dialog';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class AbandonarCotizacionDialogService {

  constructor(public dialog: MatDialog) { }

  confirm(): Observable<any> {
    let dialogData = new ConfirmDialogModel('Abandonar Cotización', 'Esta seguro que desea abandonar esta cotización?');
    let dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: dialogData
    });

    return dialogRef.afterClosed();
  };
}
