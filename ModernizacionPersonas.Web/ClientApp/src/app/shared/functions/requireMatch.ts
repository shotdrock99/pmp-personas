import { AbstractControl } from '@angular/forms';

export function RequireMatch(control: AbstractControl) {
    let selection: any = control.value;
    if (typeof selection === 'string' && selection !== '') {
        return { incorrect: true };
    }
    return null;
}