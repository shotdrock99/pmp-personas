import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ListaCotizacionesContextMenuComponent } from './lista-cotizaciones-context-menu.component';

describe('ListaCotizacionesContextMenuComponent', () => {
  let component: ListaCotizacionesContextMenuComponent;
  let fixture: ComponentFixture<ListaCotizacionesContextMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ListaCotizacionesContextMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListaCotizacionesContextMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
