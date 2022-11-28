import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'trimzero'
})
export class TrimZeroPipe implements PipeTransform {
  transform(value: any): any {
    if (!value) {
      return '';
    }

    return Number(value).toString();
  }
}
