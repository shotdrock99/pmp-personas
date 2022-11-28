import { Pipe, PipeTransform } from '@angular/core';
import { RangoEdad } from 'src/app/models/fichatecnica';

@Pipe({
  name: 'nonemptyrangosedades'
})
export class NonEmptyRangosEdadesPipe implements PipeTransform {
  transform(items: RangoEdad[], args?: any): any {
    if (!items) {
      return items;
    }

    // filter items array, items which match and return true will be
    // kept, false will be filtered out
    let result = items.filter(item => item.cantidadAsegurados > 0);
    return result;
  }
}
