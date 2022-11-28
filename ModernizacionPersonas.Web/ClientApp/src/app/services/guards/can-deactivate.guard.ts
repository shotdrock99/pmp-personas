import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CreacionCotizacionComponent } from 'src/app/pages/creacion-cotizacion/creacion-cotizacion.component';
import { of } from 'rxjs';
import { CotizacionState } from 'src/app/models';

@Injectable({
  providedIn: 'root'
})
export class CanDeactivateGuard implements CanDeactivate<CreacionCotizacionComponent> {
  canDeactivate(
    component: CreacionCotizacionComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot) {

    // si la cotizacion est en estado completada, permite seguri el proceso
    if (!component.model.estado || component.model.estado === CotizacionState.Created) {
      return true;
    }

    // de lo contrario solicite confirmacion de abandonar la cotizacion por parte del usuario
    component.abandonarCotizacionDialogService
      .confirm()
      .subscribe(async (result: boolean) => {
        if (result) {
          component.reset();
          window.location.reload();
          return of(result);
        }
      });
  }
}
