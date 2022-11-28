import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FichaTecnicaPerfilValoresComponent } from './ficha-tecnica-perfil-valores.component';

describe('FichaTecnicaPerfilValoresComponent', () => {
  let component: FichaTecnicaPerfilValoresComponent;
  let fixture: ComponentFixture<FichaTecnicaPerfilValoresComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FichaTecnicaPerfilValoresComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FichaTecnicaPerfilValoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
