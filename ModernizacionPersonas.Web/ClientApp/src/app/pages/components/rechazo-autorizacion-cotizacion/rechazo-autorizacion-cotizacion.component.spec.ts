import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RechazoAutorizacionCotizacionComponent } from './rechazo-autorizacion-cotizacion.component';

describe('RechazoAutorizacionCotizacionComponent', () => {
  let component: RechazoAutorizacionCotizacionComponent;
  let fixture: ComponentFixture<RechazoAutorizacionCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RechazoAutorizacionCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RechazoAutorizacionCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
