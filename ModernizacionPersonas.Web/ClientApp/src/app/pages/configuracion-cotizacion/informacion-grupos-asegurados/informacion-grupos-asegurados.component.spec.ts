import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionGruposAseguradosComponent } from './informacion-grupos-asegurados.component';

describe('InformacionGruposAseguradosComponent', () => {
  let component: InformacionGruposAseguradosComponent;
  let fixture: ComponentFixture<InformacionGruposAseguradosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacionGruposAseguradosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionGruposAseguradosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
