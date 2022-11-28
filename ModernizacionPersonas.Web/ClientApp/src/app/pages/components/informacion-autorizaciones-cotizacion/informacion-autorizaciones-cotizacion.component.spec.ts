import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionAutorizacionesCotizacionComponent } from './informacion-autorizaciones-cotizacion.component';

describe('InformacionAutorizacionesCotizacionComponent', () => {
  let component: InformacionAutorizacionesCotizacionComponent;
  let fixture: ComponentFixture<InformacionAutorizacionesCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacionAutorizacionesCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionAutorizacionesCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
