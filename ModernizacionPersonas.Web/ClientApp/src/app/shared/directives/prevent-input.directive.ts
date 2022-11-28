import { Directive, ElementRef, Renderer2, HostListener } from '@angular/core';

@Directive({
  selector: '[appPreventInput]'
})
export class PreventInputDirective {

  constructor(elem: ElementRef, renderer: Renderer2) { }

  @HostListener('keydown', ['$event'])
  onkeydown(event: any) {
    if (event.keyCode !== 8) {
      //event.preventDefault();
      return false;
    }
  }
}
