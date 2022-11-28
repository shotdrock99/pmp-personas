import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ReadonlyCotizacionToastComponent } from './readonly-cotizacion-toast.component';

describe('ReadonlyCotizacionToastComponent', () => {
  let component: ReadonlyCotizacionToastComponent;
  let fixture: ComponentFixture<ReadonlyCotizacionToastComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ReadonlyCotizacionToastComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReadonlyCotizacionToastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
