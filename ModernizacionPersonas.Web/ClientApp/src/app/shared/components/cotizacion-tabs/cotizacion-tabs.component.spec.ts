import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CotizacionTabsComponent } from './cotizacion-tabs.component';

describe('CotizacionTabsComponent', () => {
  let component: CotizacionTabsComponent;
  let fixture: ComponentFixture<CotizacionTabsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CotizacionTabsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CotizacionTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
