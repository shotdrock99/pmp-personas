import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CotizacionesFilterPanelComponent } from './cotizaciones-filter-panel.component';

describe('CotizacionesFilterPanelComponent', () => {
  let component: CotizacionesFilterPanelComponent;
  let fixture: ComponentFixture<CotizacionesFilterPanelComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CotizacionesFilterPanelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CotizacionesFilterPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
