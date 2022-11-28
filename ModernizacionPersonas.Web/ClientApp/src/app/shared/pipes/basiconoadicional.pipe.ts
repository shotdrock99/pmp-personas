import { Pipe, PipeTransform } from '@angular/core';
import { Amparo } from 'src/app/models';

@Pipe({
  name: 'basiconoadicional'
})
export class BasiconoadicionalPipe implements PipeTransform {
  transform(items: Amparo[], args?: any): any {
    if (!items) {
      return items;
    }

    // filter items array, items which match and return true will be
    // kept, false will be filtered out
    return items.filter(item => !item.siNoAdicional && item.siNoBasico);
  }
}
