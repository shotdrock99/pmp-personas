import { AuthenticationService } from './authentication.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpClient, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { tap, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AddTokenInterceptor implements HttpInterceptor {
  constructor(private httpClient: HttpClient,
    private authService: AuthenticationService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem("token");

    let jsonReq: HttpRequest<any> = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      },
      // setParams : {
      //   Version: '1000'
      // }
    });

    return next.handle(jsonReq).pipe(
      tap(evt => {
        if (evt instanceof HttpResponse) {
          if (evt.body && evt.status === 200) {
            // TODO handle response
          }
        }
      }, (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status !== 401) {
            return;
          }

          this.logout();
        }
      })
    )
  }

  logout() {
    const url = environment.appSettings.BASEHREF + '/login';
    window.location.href = url;
  }
}
