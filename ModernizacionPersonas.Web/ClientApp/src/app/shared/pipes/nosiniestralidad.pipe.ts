import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'nosiniestralidad',
  pure: false
})
export class NoSiniestralidadPipe implements PipeTransform {
  transform(items: any[], filter: Object): any {
    if (!items) {
      return items;
    }

    // filter items array, items which match and return true will be
    // kept, false will be filtered out
    let result = items.filter(item => !item.esProyeccionSiniestralidad);
    return result;
  }
}
