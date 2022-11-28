import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfigurarGrupoAseguradoComponent } from './configurar-grupo-asegurado.component';

describe('ConfigurarGrupoAseguradoComponent', () => {
  let component: ConfigurarGrupoAseguradoComponent;
  let fixture: ComponentFixture<ConfigurarGrupoAseguradoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurarGrupoAseguradoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurarGrupoAseguradoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
