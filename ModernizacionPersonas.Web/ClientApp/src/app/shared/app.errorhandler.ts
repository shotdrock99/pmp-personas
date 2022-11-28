import { ErrorHandler, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AlertDialogModel, AlertDialogComponent } from './components/alert-dialog';

@Injectable()
export class AppErrorHandler implements ErrorHandler {
  constructor(private dialog: MatDialog) {
  }

  handleError(error) {
    console.error(error);
  }
}
