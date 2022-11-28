import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CerrarCotizacionDialogComponent } from './cerrar-cotizacion.component';

describe('CerrarCotizacionDialogComponent', () => {
  let component: CerrarCotizacionDialogComponent;
  let fixture: ComponentFixture<CerrarCotizacionDialogComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CerrarCotizacionDialogComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CerrarCotizacionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
