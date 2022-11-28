import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TablaValoresAseguradosDiariosComponent } from './tabla-valores-asegurados-diarios.component';

describe('TablaValoresAseguradosDiariosComponent', () => {
  let component: TablaValoresAseguradosDiariosComponent;
  let fixture: ComponentFixture<TablaValoresAseguradosDiariosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TablaValoresAseguradosDiariosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TablaValoresAseguradosDiariosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
