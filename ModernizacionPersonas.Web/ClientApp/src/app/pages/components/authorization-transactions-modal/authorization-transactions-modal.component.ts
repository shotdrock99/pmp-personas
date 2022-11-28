import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ApplicationUser } from 'src/app/models/application-user';
import { CotizacionTransaction } from 'src/app/models/cotizacion-authorization';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TransactionsReaderService } from 'src/app/services/transactions-reader.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-authorization-transactions-modal',
  templateUrl: './authorization-transactions-modal.component.html',
  styleUrls: ['./authorization-transactions-modal.component.scss']
})
export class AuthorizationTransactionsModalComponent implements OnInit {

  constructor(private authenticationService: AuthenticationService,
    private transactionsReader: TransactionsReaderService,
    public dialogRef: MatDialogRef<AuthorizationTransactionsModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data2: any) {
    this.codigoCotizacion = this.data2.codigoCotizacion;
    this.version = this.data2.version;
  }

  URL_BASE = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  loggedUser: ApplicationUser;
  codigoCotizacion: number;
  version: number;
  data: CotizacionTransaction[] = [];

  ngOnInit() {
    this.loadData();

    this.loggedUser = this.authenticationService.currentUserValue;
  }

  private loadData() {
    this.transactionsReader.getAuthorizationTransactions(this.codigoCotizacion, this.version)
      .subscribe(transactions => {
        transactions.forEach(x => {
          x.initials = x.codigoUsuario.substr(0, 2).toUpperCase();
          x.downloadUrl = `${this.URL_BASE}/${this.codigoCotizacion}/authorizations/soportes/${x.codigoTransaccion}/download?version=${this.version}`;
          this.data.push(x);
        });
      })
  }

  downloadAttachment(message: any) {
    const link = document.createElement('a');
    link.href = message.downloadUrl;
    document.body.appendChild(link);
    link.click();
  }

  onDismiss(): void {
    this.dialogRef.close(true);
  }
}
