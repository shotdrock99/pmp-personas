import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfiguracionSlipAsegurabilidadComponent } from './configuracion-slip-asegurabilidad.component';

describe('ConfiguracionSlipAsegurabilidadComponent', () => {
  let component: ConfiguracionSlipAsegurabilidadComponent;
  let fixture: ComponentFixture<ConfiguracionSlipAsegurabilidadComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfiguracionSlipAsegurabilidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfiguracionSlipAsegurabilidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
