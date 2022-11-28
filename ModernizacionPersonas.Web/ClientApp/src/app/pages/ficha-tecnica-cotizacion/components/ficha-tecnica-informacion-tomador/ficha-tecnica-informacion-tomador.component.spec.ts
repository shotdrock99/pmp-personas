import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaInformacionTomadorComponent } from './ficha-tecnica-informacion-tomador.component';

describe('FichaTecnicaInformacionTomadorComponent', () => {
  let component: FichaTecnicaInformacionTomadorComponent;
  let fixture: ComponentFixture<FichaTecnicaInformacionTomadorComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaInformacionTomadorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaInformacionTomadorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
