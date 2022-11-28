import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaPerfilEdadesComponent } from './ficha-tecnica-perfil-edades.component';

describe('FichaTecnicaPerfilEdadesComponent', () => {
  let component: FichaTecnicaPerfilEdadesComponent;
  let fixture: ComponentFixture<FichaTecnicaPerfilEdadesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaPerfilEdadesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaPerfilEdadesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
