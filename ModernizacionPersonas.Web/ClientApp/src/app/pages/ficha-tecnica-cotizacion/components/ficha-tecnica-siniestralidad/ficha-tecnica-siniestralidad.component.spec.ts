import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaSiniestralidadComponent } from './ficha-tecnica-siniestralidad.component';

describe('FichaTecnicaSiniestralidadComponent', () => {
  let component: FichaTecnicaSiniestralidadComponent;
  let fixture: ComponentFixture<FichaTecnicaSiniestralidadComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaSiniestralidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaSiniestralidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
