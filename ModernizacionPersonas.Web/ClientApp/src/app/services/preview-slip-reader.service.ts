import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { GenerarSlipDataResponse } from '../models/slip';
import { NotificationService } from '../shared/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class PreviewSlipReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService) {}

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones'])
    });

    return of(false);
  }

  consultarSlip(codigoCotizacion: number, version: number) {
    const params = new HttpParams().append('version', version.toString());

    const url = `${this.BASE_URL}/${codigoCotizacion}/slip/preview`;
    return this.httpClient.get<GenerarSlipDataResponse>(url, { params: params })
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );

  }
  ConsultarPDFSlipCotizacion(args: any) {
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/slip/previewPDF`;
    return this.httpClient.post(url, args);
  }
}
