import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ListaCotizacionesComponent } from './lista-cotizaciones.component';

describe('ListaCotizacionesComponent', () => {
  let component: ListaCotizacionesComponent;
  let fixture: ComponentFixture<ListaCotizacionesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ListaCotizacionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListaCotizacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
