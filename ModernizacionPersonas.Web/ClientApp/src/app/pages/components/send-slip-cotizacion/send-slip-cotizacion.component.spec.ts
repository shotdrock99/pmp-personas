import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SendSlipCotizacionComponent } from './send-slip-cotizacion.component';

describe('SendSlipCotizacionComponent', () => {
  let component: SendSlipCotizacionComponent;
  let fixture: ComponentFixture<SendSlipCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SendSlipCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SendSlipCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
