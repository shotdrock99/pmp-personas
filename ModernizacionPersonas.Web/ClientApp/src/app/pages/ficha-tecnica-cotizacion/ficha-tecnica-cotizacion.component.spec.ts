import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaCotizacionComponent } from './ficha-tecnica-cotizacion.component';

describe('FichaTecnicaCotizacionComponent', () => {
  let component: FichaTecnicaCotizacionComponent;
  let fixture: ComponentFixture<FichaTecnicaCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
