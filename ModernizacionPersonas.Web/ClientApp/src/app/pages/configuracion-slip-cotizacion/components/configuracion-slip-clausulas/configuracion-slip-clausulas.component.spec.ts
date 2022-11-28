import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfiguracionSlipClausulasComponent } from './configuracion-slip-clausulas.component';

describe('SlipClausulasComponent', () => {
  let component: ConfiguracionSlipClausulasComponent;
  let fixture: ComponentFixture<ConfiguracionSlipClausulasComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfiguracionSlipClausulasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfiguracionSlipClausulasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
