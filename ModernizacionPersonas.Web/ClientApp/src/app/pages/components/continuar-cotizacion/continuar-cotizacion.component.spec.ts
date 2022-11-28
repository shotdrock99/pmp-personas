import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ContinuarCotizacionComponent } from './continuar-cotizacion.component';

describe('ContinuarCotizacionComponent', () => {
  let component: ContinuarCotizacionComponent;
  let fixture: ComponentFixture<ContinuarCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ContinuarCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContinuarCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
