import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { Cotizacion } from '../models';
import { CotizacionPersistenceService } from './../services/cotizacion-persistence.service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CotizacionDataResolver implements Resolve<Cotizacion> {

  constructor(private router: Router,
    private cotizacionDataService: CotizacionPersistenceService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Cotizacion | Observable<Cotizacion> | Promise<Cotizacion> {
    const param0 = route.paramMap.get('cod_cotiza');
    const param1 = route.queryParams.version;
    const resolveDependencies = route.data.resolveDependencies;

    const codigoCotizacion = Number(param0);
    if (!isNaN(codigoCotizacion)) {
      const version = Number(param1) || 0;
      return this.cotizacionDataService.resolveCotizacionData(codigoCotizacion, version, resolveDependencies)
        .then(res => res);
    }

    this.router.navigate(['/cotizaciones']);
  }
}
