import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, CanDeactivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { ConsultaCotizacionComponent } from 'src/app/pages/consulta-cotizacion/consulta-cotizacion.component';

@Injectable({
  providedIn: 'root'
})
export class UnlockCotizacionGuard implements CanDeactivate<ConsultaCotizacionComponent> {
  canDeactivate(
    component: ConsultaCotizacionComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot) {

    component.triggerUnlockCotizacion();
    return true;
  }

}
