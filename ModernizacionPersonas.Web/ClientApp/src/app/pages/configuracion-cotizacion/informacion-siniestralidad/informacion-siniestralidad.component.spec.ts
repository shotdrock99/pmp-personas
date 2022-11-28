import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionSiniestralidadComponent } from './informacion-siniestralidad.component';

describe('InformacionSiniestralidadComponent', () => {
  let component: InformacionSiniestralidadComponent;
  let fixture: ComponentFixture<InformacionSiniestralidadComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacionSiniestralidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionSiniestralidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
