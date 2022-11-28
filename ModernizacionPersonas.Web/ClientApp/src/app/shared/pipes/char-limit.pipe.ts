import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'charLimit'
})
export class CharLimitPipe implements PipeTransform {

  transform(val: any, limit: number): any {
    if (val) {
      const value = val.trim();
      let substr = value.substr(0, limit);
      if (value.length > limit) {
        return `${substr}...`;
      }
    }

    return val;
  }
}
