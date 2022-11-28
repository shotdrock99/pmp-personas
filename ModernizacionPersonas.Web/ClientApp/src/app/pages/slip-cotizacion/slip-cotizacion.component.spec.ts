import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipCotizacionComponent } from './slip-cotizacion.component';

describe('SlipCotizacionComponent', () => {
  let component: SlipCotizacionComponent;
  let fixture: ComponentFixture<SlipCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipCotizacionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
