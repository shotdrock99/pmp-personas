import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'soliPercentageMask'
})
export class SoliPercentageMaskPipe implements PipeTransform {
  transform(value: any, limit: number): any {
    if (!value) {
      return '';
    }

    return value + '%';
  }

}
