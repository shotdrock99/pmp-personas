import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditarNombreGrupoAseguradoComponent } from './editar-nombre-grupo-asegurado.component';

describe('EditarNombreGrupoAseguradoComponent', () => {
  let component: EditarNombreGrupoAseguradoComponent;
  let fixture: ComponentFixture<EditarNombreGrupoAseguradoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditarNombreGrupoAseguradoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditarNombreGrupoAseguradoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
