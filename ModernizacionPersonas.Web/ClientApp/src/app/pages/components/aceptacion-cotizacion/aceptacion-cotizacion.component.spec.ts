import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AceptacionCotizacionDialogComponent } from './aceptacion-cotizacion.component';

describe('AceptacionCotizacionDialogComponent', () => {
  let component: AceptacionCotizacionDialogComponent;
  let fixture: ComponentFixture<AceptacionCotizacionDialogComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AceptacionCotizacionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AceptacionCotizacionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
