import { Directive, EventEmitter, HostListener, Output } from '@angular/core';
@Directive({
  selector: '[uppercase]'
})

export class UppercaseDirective {
  @Output() 
  ngModelChange: EventEmitter<any> = new EventEmitter();
  value: any;

  @HostListener('input', ['$event'])
  onInputChange(event:any) {
    this.value = event.target.value.toUpperCase();
    event.target.value = this.value;
  }
}