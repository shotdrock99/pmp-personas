import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ResumenCotizacionComponent } from './resumen-cotizacion.component';

describe('ResumenCotizacionComponent', () => {
  let component: ResumenCotizacionComponent;
  let fixture: ComponentFixture<ResumenCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ResumenCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResumenCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
