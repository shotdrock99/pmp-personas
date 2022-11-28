import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'input-loading',
  template: `<div *ngIf="loading" class="thin-text sm"><mat-spinner></mat-spinner> Buscando...</div>`,
  styles: [`div { position: absolute;right: 0;top: 10px; color:blue; font-weight:700; }`]
})
export class InputLoadingComponent implements OnInit {

  @Input('loading')
  loading: boolean = false;

  constructor() { }

  ngOnInit() {
  }

}
