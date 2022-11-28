import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipEdadesAsegurabilidadComponent } from './slip-edades-asegurabilidad.component';

describe('SlipEdadesAsegurabilidadComponent', () => {
  let component: SlipEdadesAsegurabilidadComponent;
  let fixture: ComponentFixture<SlipEdadesAsegurabilidadComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipEdadesAsegurabilidadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipEdadesAsegurabilidadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
