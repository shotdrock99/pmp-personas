import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuthorizeCotizacionComponent } from './authorize-cotizacion.component';

describe('AuthorizeCotizacionComponent', () => {
  let component: AuthorizeCotizacionComponent;
  let fixture: ComponentFixture<AuthorizeCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuthorizeCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthorizeCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
