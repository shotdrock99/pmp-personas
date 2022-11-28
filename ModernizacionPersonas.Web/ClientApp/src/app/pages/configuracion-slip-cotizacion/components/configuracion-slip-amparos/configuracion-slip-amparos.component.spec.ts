import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfiguracionSlipAmparosComponent } from './configuracion-slip-amparos.component';

describe('SlipAmparosComponent', () => {
  let component: ConfiguracionSlipAmparosComponent;
  let fixture: ComponentFixture<ConfiguracionSlipAmparosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfiguracionSlipAmparosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfiguracionSlipAmparosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
