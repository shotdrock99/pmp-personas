import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AgregarGrupoAseguradoComponent } from './agregar-grupo-asegurado.component';

describe('AgregarGrupoAseguradoComponent', () => {
  let component: AgregarGrupoAseguradoComponent;
  let fixture: ComponentFixture<AgregarGrupoAseguradoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AgregarGrupoAseguradoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgregarGrupoAseguradoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
