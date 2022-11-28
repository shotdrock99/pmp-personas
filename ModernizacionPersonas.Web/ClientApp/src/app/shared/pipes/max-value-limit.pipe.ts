import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'maxValueLimit'
})
export class MaxValueLimitPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let maxValue = args;
    let parsedValue = Number.parseInt(value);
    if (parsedValue && parsedValue > maxValue) {
      return '200%';
    }

    return value;
  }

}
