import { Pipe, PipeTransform } from '@angular/core';
import { Amparo } from 'src/app/models';

@Pipe({
  name: 'amparosbasicos'
})
export class AmparosbasicosPipe implements PipeTransform {
  transform(items: Amparo[], args?: any): any {
    if (!items) {
      return items;
    }

    // filter items array, items which match and return true will be
    // kept, false will be filtered out
    let tmp = items.filter(item => item.codigoAmparo !== 95);
    let ascList = tmp.sort((a, b) => a.codigoAmparo - b.codigoAmparo)
    // sort by codigoGrupoAsegurado y siNoAdicional
    let result = ascList.sort((a, b) => (a.codigoGrupoAmparo < b.codigoGrupoAmparo) ? -1 : a.siNoAdicional < b.siNoAdicional ? -1 : 0);
    return result;
  }
}
