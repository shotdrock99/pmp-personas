import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ListaCotizacionesAutorizacionComponent } from './lista-cotizaciones-autorizacion.component';

describe('ListaCotizacionesAutorizacionComponent', () => {
  let component: ListaCotizacionesAutorizacionComponent;
  let fixture: ComponentFixture<ListaCotizacionesAutorizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ListaCotizacionesAutorizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListaCotizacionesAutorizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
