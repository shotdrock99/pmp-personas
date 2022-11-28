import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfiguracionSlipCotizacionComponent } from './configuracion-slip-cotizacion.component';

describe('ConfiguracionSlipCotizacionComponent', () => {
  let component: ConfiguracionSlipCotizacionComponent;
  let fixture: ComponentFixture<ConfiguracionSlipCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfiguracionSlipCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfiguracionSlipCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
