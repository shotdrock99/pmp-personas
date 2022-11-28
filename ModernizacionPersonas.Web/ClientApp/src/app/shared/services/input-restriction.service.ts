import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class InputRestrictionService {

  arabicRegex = '[\u0600-\u06FF]';

  constructor() { }

  validateIntegerOnly(event) {
    const e = <KeyboardEvent>event;
    if (e.key === 'Tab' || e.key === 'TAB') {
      return;
    }
    if ([46, 8, 9, 27, 13, 110, 36, 37, 39].indexOf(e.keyCode) !== -1 ||
      // Allow: Ctrl+A
      (e.keyCode === 65 && e.ctrlKey === true) ||
      // Allow: Ctrl+C
      (e.keyCode === 67 && e.ctrlKey === true) ||
      // Allow: Ctrl+V
      (e.keyCode === 86 && e.ctrlKey === true) ||
      // Allow: Ctrl+X
      (e.keyCode === 88 && e.ctrlKey === true) ||
      // Allow: Ctrl+Z
      (e.keyCode === 90 && e.ctrlKey === true) ||
      // Allow: Shift and Home
      (e.keyCode === 36 && e.shiftKey === true) ||
      // Allow: Shift and End
      (e.keyCode === 35 && e.shiftKey === true) ||
      // Allow: Shift and Left Arrow
      (e.keyCode === 37 && e.shiftKey === true) ||
      // Allow: Shift and Rigth Arrow
      (e.keyCode === 39 && e.shiftKey === true)) {
      // let it happen, don't do anything
      return;
    }
    if (['1', '2', '3', '4', '5', '6', '7', '8', '9', '0'].indexOf(e.key) === -1) {
      e.preventDefault();
    }
  }

  validateNoSpecialChars(event) {
    
    const e = <KeyboardEvent>event;
    if (e.key === 'Tab' || e.key === 'TAB') {
      return;
    }
    let k;
    k = event.keyCode;  // k = event.charCode;  (Both can be used)
    if ((k > 64 && k < 91) || (k > 96 && k < 123) || k === 8 || k === 32 || (k >= 48 && k <= 57)) {
      return;
    }
    const ch = String.fromCharCode(e.keyCode);
    const regEx = new RegExp(this.arabicRegex);
    if (regEx.test(ch)) {
      return;
    }
    e.preventDefault();
  }
}
