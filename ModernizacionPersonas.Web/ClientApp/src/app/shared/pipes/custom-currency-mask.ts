import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'soliCurrencyMask'
})
export class SoliCurrencyMaskPipe implements PipeTransform {
  transform(value: any, limit: number): any {
    if (!value) {
      return 'N/A';
    }

    return '$ ' + value;
  }
}
