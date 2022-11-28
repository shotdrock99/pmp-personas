import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaGruposAseguradosComponent } from './ficha-tecnica-grupos-asegurados.component';

describe('FichaTecnicaGruposAseguradosComponent', () => {
  let component: FichaTecnicaGruposAseguradosComponent;
  let fixture: ComponentFixture<FichaTecnicaGruposAseguradosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaGruposAseguradosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaGruposAseguradosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
