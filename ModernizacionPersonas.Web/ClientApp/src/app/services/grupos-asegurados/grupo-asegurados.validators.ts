import { AbstractControl, FormArray, ValidatorFn } from '@angular/forms';
import { Amparo } from 'src/app/models';

export function AmparosValidator(control: FormArray) {
    let controls = control.value.filter(value => Object.keys(value).length !== 0);
    let hasAmparos = controls.length > 0;
    if (!hasAmparos) {
        return { minlength: true };
    }

    let amparosBasicosNoAdicionales = controls.filter((x: Amparo) => !x.siNoAdicional && x.siNoBasico);
    let hasAmparoBasicoNoAdicional = amparosBasicosNoAdicionales.length > 0;
    let excedeBasicoNoAdicional = amparosBasicosNoAdicionales.length > 1;

    if (!hasAmparoBasicoNoAdicional) {
        return { amparonoadicionalrequired: true }
    }

    if (excedeBasicoNoAdicional) {
        return { excedamparosbasicos: true }
    }

    return null;
}

export function GrupoAseguradosValidator(control: AbstractControl) {
    var c = control;
    return null;
}

export function ValoresAseguradosValidatorFn(control: FormArray) {
    var c = control;
    // if (control.controls.length == 0) {
    //     return { invalid: true };
    // }

    return null;
}

export function ValorAseguradoOpcionesValidator(control: FormArray) {
    var c = control;
    return null;
}

export function ValorAseguradoOpcionValidator(control: AbstractControl) {
    var c = control;
    return null;
}

export function PercentValidator(control: AbstractControl) {
    // if (control.value === 0) {
    //     return { 'invalid': true };
    // }

    return null;
}

export function RawValueValidator(control: AbstractControl) {
    // if (control.value === 0) {
    //     return { 'invalid': true };
    // }

    return null;
}

export function EdadesAmparoValidator(control: AbstractControl) {
    var c = control;
    return null;
}

export function EdadesAmparosValidator(control: AbstractControl) {
    var c = control;
    return null;
}