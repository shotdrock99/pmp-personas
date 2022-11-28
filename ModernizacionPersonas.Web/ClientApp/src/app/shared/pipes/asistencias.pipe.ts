import { Pipe, PipeTransform } from '@angular/core';
import { Amparo } from 'src/app/models';

@Pipe({
  name: 'asistencias'
})
export class AsistenciasPipe implements PipeTransform {

  transform(items: Amparo[], args?: any): any {
    if (!items) {
      return items;
    }

    // filter items array, items which match and return true will be
    // kept, false will be filtered out
    let result = items.filter(item => item.codigoAmparo === 95);
    return result;
  }

}
